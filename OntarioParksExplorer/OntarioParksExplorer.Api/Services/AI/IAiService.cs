using OntarioParksExplorer.Api.Data.Entities;
using OntarioParksExplorer.Api.Models.DTOs.AI;

namespace OntarioParksExplorer.Api.Services.AI;

/// <summary>
/// Service interface for AI-powered park features including summaries, recommendations, chat, and visit planning.
/// </summary>
public interface IAiService
{
    /// <summary>
    /// Generates an AI-powered summary for a park highlighting key features and activities.
    /// Results are cached for 24 hours to improve performance.
    /// </summary>
    /// <param name="park">The park entity to summarize</param>
    /// <returns>A 2-3 sentence engaging summary, or a fallback message if AI is unavailable</returns>
    Task<string> GenerateParkSummaryAsync(Park park);

    /// <summary>
    /// Gets personalized park recommendations based on user preferences.
    /// </summary>
    /// <param name="request">User preferences including activities, region, and additional criteria</param>
    /// <returns>A list of up to 5 recommended parks with match scores and reasons</returns>
    Task<List<ParkRecommendation>> GetRecommendationsAsync(RecommendationRequest request);

    /// <summary>
    /// Handles conversational chat about Ontario parks with context-aware responses.
    /// Optimizes token usage by loading only relevant park data based on the question.
    /// </summary>
    /// <param name="request">Chat request containing the user message and optional conversation history</param>
    /// <returns>AI-generated response to the user's question</returns>
    Task<string> ChatAsync(ChatRequest request);

    /// <summary>
    /// Creates a personalized multi-day visit itinerary for a specific park.
    /// </summary>
    /// <param name="request">Visit plan request containing park ID, duration, interests, and season</param>
    /// <returns>Day-by-day itinerary with activities and practical tips</returns>
    Task<VisitPlan> PlanVisitAsync(VisitPlanRequest request);
}
