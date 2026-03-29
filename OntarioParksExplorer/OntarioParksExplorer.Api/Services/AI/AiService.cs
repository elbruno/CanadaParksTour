using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Data.Entities;
using OntarioParksExplorer.Api.Models.DTOs.AI;
using OntarioParksExplorer.Api.Services.AI.Prompts;

namespace OntarioParksExplorer.Api.Services.AI;

/// <summary>
/// AI service providing park summaries, recommendations, chat, and visit planning.
/// Uses Microsoft Agent Framework with GitHub Copilot SDK as the AI backend.
/// Implements caching and optimized context injection for token efficiency.
/// </summary>
public class AiService : IAiService
{
    private readonly ICopilotAgentProvider _agentProvider;
    private readonly ParksDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AiService> _logger;
    
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromHours(24);
    private static readonly TimeSpan AiTimeout = TimeSpan.FromSeconds(30);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiService(
        ICopilotAgentProvider agentProvider, 
        ParksDbContext context, 
        IMemoryCache cache,
        ILogger<AiService> logger)
    {
        _agentProvider = agentProvider;
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Generates an AI-powered summary for a park. Results are cached for 24 hours.
    /// </summary>
    public async Task<string> GenerateParkSummaryAsync(Park park)
    {
        var agent = await _agentProvider.GetAgentAsync();
        if (agent == null)
        {
            return "AI features are not configured. Ensure GitHub Copilot CLI is installed and authenticated to enable AI-powered summaries.";
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

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await agent.RunAsync(prompt).WaitAsync(cts.Token);

            var summary = response?.ToString() ?? "Unable to generate summary at this time.";
            
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
        var agent = await _agentProvider.GetAgentAsync();
        if (agent == null)
        {
            return new List<ParkRecommendation>
            {
                new()
                {
                    ParkId = 0,
                    ParkName = "AI Not Configured",
                    Reason = "AI features are not configured. Ensure GitHub Copilot CLI is installed and authenticated to enable recommendations.",
                    MatchScore = 0.0
                }
            };
        }

        try
        {
            var prompt = PromptTemplates.Recommendations(request.Activities, request.Region, request.PreferenceText);

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await agent.RunAsync(prompt).WaitAsync(cts.Token);

            var jsonResponse = response?.ToString() ?? "{}";
            
            // Extract JSON from response (agent may include extra text around JSON)
            jsonResponse = ExtractJson(jsonResponse);
            
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
    /// Embeds conversation history in the prompt for stateless multi-turn support.
    /// </summary>
    public async Task<string> ChatAsync(ChatRequest request)
    {
        var agent = await _agentProvider.GetAgentAsync();
        if (agent == null)
        {
            return "AI features are not configured. Ensure GitHub Copilot CLI is installed and authenticated to enable the chat feature.";
        }

        try
        {
            var parkContext = await BuildMinimalParkContextAsync(request.Message);

            var promptParts = new List<string>();

            // Embed conversation history in the prompt
            if (request.ConversationHistory != null && request.ConversationHistory.Any())
            {
                promptParts.Add("Previous conversation:");
                foreach (var msg in request.ConversationHistory.TakeLast(10))
                {
                    var role = msg.Role == "user" ? "User" : "Assistant";
                    promptParts.Add($"{role}: {msg.Content}");
                }
                promptParts.Add("");
            }

            promptParts.Add(PromptTemplates.Chat(request.Message, parkContext));

            var fullPrompt = string.Join("\n", promptParts);

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await agent.RunAsync(fullPrompt).WaitAsync(cts.Token);

            return response?.ToString() ?? "I'm unable to provide an answer at this time.";
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

        var agent = await _agentProvider.GetAgentAsync();
        if (agent == null)
        {
            return new VisitPlan
            {
                ParkName = park.Name,
                DurationDays = request.DurationDays,
                Days = new List<VisitDay>(),
                Tips = new List<string>
                {
                    "AI features are not configured. Ensure GitHub Copilot CLI is installed and authenticated to enable visit planning."
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

            using var cts = new CancellationTokenSource(AiTimeout);
            var response = await agent.RunAsync(prompt).WaitAsync(cts.Token);

            var jsonResponse = response?.ToString() ?? "{}";
            
            // Extract JSON from response (agent may include extra text around JSON)
            jsonResponse = ExtractJson(jsonResponse);
            
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

    /// <summary>
    /// Extracts JSON from a response that may contain surrounding text.
    /// The Copilot agent may wrap JSON in markdown code blocks or add explanatory text.
    /// </summary>
    private static string ExtractJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "{}";

        // Try to extract from markdown code block
        var jsonBlockStart = text.IndexOf("```json", StringComparison.OrdinalIgnoreCase);
        if (jsonBlockStart >= 0)
        {
            var contentStart = text.IndexOf('\n', jsonBlockStart) + 1;
            var blockEnd = text.IndexOf("```", contentStart, StringComparison.Ordinal);
            if (blockEnd > contentStart)
            {
                return text[contentStart..blockEnd].Trim();
            }
        }

        // Try to extract from generic code block
        var codeBlockStart = text.IndexOf("```", StringComparison.Ordinal);
        if (codeBlockStart >= 0)
        {
            var contentStart = text.IndexOf('\n', codeBlockStart) + 1;
            var blockEnd = text.IndexOf("```", contentStart, StringComparison.Ordinal);
            if (blockEnd > contentStart)
            {
                return text[contentStart..blockEnd].Trim();
            }
        }

        // Try to find raw JSON object or array
        var firstBrace = text.IndexOf('{');
        var firstBracket = text.IndexOf('[');
        
        if (firstBrace >= 0)
        {
            var lastBrace = text.LastIndexOf('}');
            if (lastBrace > firstBrace)
            {
                return text[firstBrace..(lastBrace + 1)];
            }
        }

        if (firstBracket >= 0)
        {
            var lastBracket = text.LastIndexOf(']');
            if (lastBracket > firstBracket)
            {
                return text[firstBracket..(lastBracket + 1)];
            }
        }

        return text.Trim();
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
