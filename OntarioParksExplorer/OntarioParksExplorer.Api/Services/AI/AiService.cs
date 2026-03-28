using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Data.Entities;
using OntarioParksExplorer.Api.Models.DTOs.AI;
using OntarioParksExplorer.Api.Services.AI.Prompts;
using AIChatMessage = Microsoft.Extensions.AI.ChatMessage;
using DTOChatMessage = OntarioParksExplorer.Api.Models.DTOs.AI.ChatMessage;

namespace OntarioParksExplorer.Api.Services.AI;

/// <summary>
/// AI service providing park summaries, recommendations, chat, and visit planning.
/// Implements caching and optimized context injection for token efficiency.
/// </summary>
public class AiService : IAiService
{
    private readonly IChatClient? _chatClient;
    private readonly ParksDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AiService> _logger;
    private readonly bool _isConfigured;
    
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromHours(24);
    private static readonly TimeSpan AiTimeout = TimeSpan.FromSeconds(30);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiService(
        IChatClient? chatClient, 
        ParksDbContext context, 
        IMemoryCache cache,
        ILogger<AiService> logger)
    {
        _chatClient = chatClient;
        _context = context;
        _cache = cache;
        _logger = logger;
        _isConfigured = _chatClient != null;
    }

    /// <summary>
    /// Generates an AI-powered summary for a park. Results are cached for 24 hours.
    /// </summary>
    public async Task<string> GenerateParkSummaryAsync(Park park)
    {
        if (!_isConfigured)
        {
            return "AI features are not configured. Set the AI:ApiKey in appsettings.json to enable AI-powered summaries.";
        }

        var cacheKey = $"park-summary-{park.Id}";
        
        if (_cache.TryGetValue<string>(cacheKey, out var cachedSummary))
        {
            _logger.LogDebug("Returning cached summary for park {ParkId}", park.Id);
            return cachedSummary!;
        }

        try
        {
            var activities = await _context.ParkActivities
                .Where(pa => pa.ParkId == park.Id)
                .Include(pa => pa.Activity)
                .Select(pa => pa.Activity.Name)
                .Take(10)
                .ToListAsync();

            var prompt = PromptTemplates.ParkSummary(park.Name, park.Description, activities);

            var messages = new List<AIChatMessage>
            {
                new AIChatMessage(ChatRole.User, prompt)
            };

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await _chatClient!.GetResponseAsync(messages, cancellationToken: cts.Token);

            var summary = response.Text ?? "Unable to generate summary at this time.";
            
            _cache.Set(cacheKey, summary, DefaultCacheDuration);
            _logger.LogInformation("Generated and cached summary for park {ParkId}", park.Id);
            
            return summary;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout generating park summary for {ParkName}", park.Name);
            return $"A detailed overview of {park.Name} with its unique features and activities.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating park summary for {ParkName}", park.Name);
            return $"A detailed overview of {park.Name} with its unique features and activities.";
        }
    }

    /// <summary>
    /// Gets personalized park recommendations based on user preferences.
    /// </summary>
    public async Task<List<ParkRecommendation>> GetRecommendationsAsync(RecommendationRequest request)
    {
        if (!_isConfigured)
        {
            return new List<ParkRecommendation>
            {
                new()
                {
                    ParkId = 0,
                    ParkName = "AI Not Configured",
                    Reason = "AI features are not configured. Set the AI:ApiKey in appsettings.json to enable recommendations.",
                    MatchScore = 0.0
                }
            };
        }

        try
        {
            var prompt = PromptTemplates.Recommendations(request.Activities, request.Region, request.PreferenceText);

            var messages = new List<AIChatMessage>
            {
                new AIChatMessage(ChatRole.User, prompt)
            };

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await _chatClient!.GetResponseAsync(messages, new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.Json
            }, cts.Token);

            var jsonResponse = response.Text ?? "{}";
            var result = JsonSerializer.Deserialize<RecommendationResponse>(jsonResponse, JsonOptions);

            if (result?.Recommendations == null || !result.Recommendations.Any())
            {
                _logger.LogWarning("No recommendations returned from AI");
                return new List<ParkRecommendation>();
            }

            var recommendations = new List<ParkRecommendation>();
            foreach (var rec in result.Recommendations.Take(5))
            {
                var park = await _context.Parks
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == rec.ParkName.ToLower());

                if (park != null)
                {
                    recommendations.Add(new ParkRecommendation
                    {
                        ParkId = park.Id,
                        ParkName = park.Name,
                        Reason = rec.Reason,
                        MatchScore = Math.Clamp(rec.MatchScore, 0.0, 1.0)
                    });
                }
                else
                {
                    _logger.LogDebug("Park not found in database: {ParkName}", rec.ParkName);
                }
            }

            return recommendations;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout getting recommendations");
            return new List<ParkRecommendation>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse AI recommendation response");
            return new List<ParkRecommendation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations");
            return new List<ParkRecommendation>();
        }
    }

    /// <summary>
    /// Handles chat interactions about Ontario parks with context-aware responses.
    /// Only loads minimal, relevant park data to optimize token usage.
    /// </summary>
    public async Task<string> ChatAsync(ChatRequest request)
    {
        if (!_isConfigured)
        {
            return "AI features are not configured. Set the AI:ApiKey in appsettings.json to enable the chat feature.";
        }

        try
        {
            var parkContext = await BuildMinimalParkContextAsync(request.Message);

            var messages = new List<AIChatMessage>();

            if (request.ConversationHistory != null && request.ConversationHistory.Any())
            {
                foreach (var msg in request.ConversationHistory.TakeLast(10))
                {
                    messages.Add(new AIChatMessage(
                        msg.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                        msg.Content
                    ));
                }
            }

            messages.Add(new AIChatMessage(
                ChatRole.User,
                PromptTemplates.Chat(request.Message, parkContext)
            ));

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await _chatClient!.GetResponseAsync(messages, cancellationToken: cts.Token);

            return response.Text ?? "I'm unable to provide an answer at this time.";
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout in chat");
            return "I'm taking longer than expected to respond. Please try rephrasing your question.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat");
            return "I'm having trouble processing your request. Please try again.";
        }
    }

    /// <summary>
    /// Builds minimal park context based on the user's question to optimize token usage.
    /// </summary>
    private async Task<string> BuildMinimalParkContextAsync(string question)
    {
        var questionLower = question.ToLower();
        var limit = 5;

        IQueryable<Park> query = _context.Parks
            .Include(p => p.ParkActivities)
            .ThenInclude(pa => pa.Activity);

        if (questionLower.Contains("north"))
            query = query.Where(p => p.Region.ToLower().Contains("north"));
        else if (questionLower.Contains("south"))
            query = query.Where(p => p.Region.ToLower().Contains("south"));
        else if (questionLower.Contains("east"))
            query = query.Where(p => p.Region.ToLower().Contains("east"));
        else if (questionLower.Contains("west"))
            query = query.Where(p => p.Region.ToLower().Contains("west"));

        var parks = await query.Take(limit).ToListAsync();

        if (!parks.Any())
        {
            parks = await _context.Parks
                .Include(p => p.ParkActivities)
                .ThenInclude(pa => pa.Activity)
                .Take(limit)
                .ToListAsync();
        }

        return string.Join("\n\n", parks.Select(p =>
            $"{p.Name} ({p.Region}): {TruncateDescription(p.Description, 100)}\n" +
            $"Activities: {string.Join(", ", p.ParkActivities.Take(5).Select(pa => pa.Activity.Name))}"
        ));
    }

    private static string TruncateDescription(string description, int maxLength)
    {
        if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
            return description;
        
        return description.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// Creates a personalized visit plan for a specific park.
    /// </summary>
    public async Task<VisitPlan> PlanVisitAsync(VisitPlanRequest request)
    {
        var park = await _context.Parks
            .Include(p => p.ParkActivities)
            .ThenInclude(pa => pa.Activity)
            .FirstOrDefaultAsync(p => p.Id == request.ParkId);

        if (park == null)
        {
            throw new ArgumentException($"Park with ID {request.ParkId} not found");
        }

        if (!_isConfigured)
        {
            return new VisitPlan
            {
                ParkName = park.Name,
                DurationDays = request.DurationDays,
                Days = new List<VisitDay>(),
                Tips = new List<string>
                {
                    "AI features are not configured. Set the AI:ApiKey in appsettings.json to enable visit planning."
                }
            };
        }

        try
        {
            var activities = park.ParkActivities.Select(pa => pa.Activity.Name).ToList();
            var prompt = PromptTemplates.VisitPlan(
                park.Name,
                park.Description,
                activities,
                request.DurationDays,
                request.Interests,
                request.Season
            );

            var messages = new List<AIChatMessage>
            {
                new AIChatMessage(ChatRole.User, prompt)
            };

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await _chatClient!.GetResponseAsync(messages, new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.Json
            }, cts.Token);

            var jsonResponse = response.Text ?? "{}";
            var result = JsonSerializer.Deserialize<VisitPlanResponse>(jsonResponse, JsonOptions);

            if (result?.Days == null || !result.Days.Any())
            {
                _logger.LogWarning("AI returned empty visit plan for park {ParkId}", park.Id);
                return CreateFallbackPlan(park, request.DurationDays);
            }

            return new VisitPlan
            {
                ParkName = park.Name,
                DurationDays = request.DurationDays,
                Days = result.Days,
                Tips = result.Tips ?? new List<string>()
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout planning visit for {ParkName}", park.Name);
            return CreateFallbackPlan(park, request.DurationDays);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse AI visit plan response");
            return CreateFallbackPlan(park, request.DurationDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error planning visit for {ParkName}", park.Name);
            return CreateFallbackPlan(park, request.DurationDays);
        }
    }

    private static VisitPlan CreateFallbackPlan(Park park, int durationDays)
    {
        return new VisitPlan
        {
            ParkName = park.Name,
            DurationDays = durationDays,
            Days = new List<VisitDay>(),
            Tips = new List<string> 
            { 
                "Visit planning is temporarily unavailable. Please try again later.",
                $"Check {park.Name}'s official website for the latest information."
            }
        };
    }

    private class RecommendationResponse
    {
        public List<RecommendationItem> Recommendations { get; set; } = new();
    }

    private class RecommendationItem
    {
        public string ParkName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public double MatchScore { get; set; }
    }

    private class VisitPlanResponse
    {
        public List<VisitDay> Days { get; set; } = new();
        public List<string> Tips { get; set; } = new();
    }
}
