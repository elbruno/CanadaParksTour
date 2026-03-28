namespace OntarioParksExplorer.Api.Services.AI.Prompts;

/// <summary>
/// Centralized prompt templates for AI-powered features.
/// All templates are optimized for token efficiency and reliable output formatting.
/// </summary>
public static class PromptTemplates
{
    /// <summary>
    /// Generates a prompt for creating a concise, engaging park summary.
    /// Output: 2-3 sentence plain text summary.
    /// </summary>
    /// <param name="parkName">Name of the park</param>
    /// <param name="description">Brief park description</param>
    /// <param name="activities">List of available activities (max 10 recommended)</param>
    public static string ParkSummary(string parkName, string description, List<string> activities)
    {
        var activitiesList = activities.Take(10).Any() 
            ? string.Join(", ", activities.Take(10)) 
            : "various outdoor activities";
        
        return $@"Summarize {parkName} in 2-3 engaging sentences.

Context:
- Description: {description}
- Activities: {activitiesList}

Focus on unique features and visitor appeal. Be enthusiastic yet informative.";
    }

    /// <summary>
    /// Generates a prompt for personalized park recommendations.
    /// Output: JSON with array of recommendations (parkName, reason, matchScore).
    /// </summary>
    /// <param name="activities">Preferred activities</param>
    /// <param name="region">Optional region filter</param>
    /// <param name="preferenceText">Optional additional preferences</param>
    public static string Recommendations(List<string> activities, string? region, string? preferenceText)
    {
        var activityList = activities.Any() ? string.Join(", ", activities) : "any";
        var regionFilter = !string.IsNullOrEmpty(region) ? $" in {region}" : "";
        var preferences = !string.IsNullOrEmpty(preferenceText) ? $"\nPreferences: {preferenceText}" : "";

        return $@"Recommend up to 5 Ontario Provincial Parks matching these criteria:

Activities: {activityList}
Region{regionFilter}{preferences}

JSON format:
{{""recommendations"":[{{""parkName"":"""",""reason"":"""",""matchScore"":0.95}}]}}

Keep reasons to 1-2 sentences. Match scores 0.0-1.0 based on criteria fit.";
    }

    /// <summary>
    /// Generates a prompt for the Q and A chatbot feature.
    /// Output: Plain text response to user's question.
    /// </summary>
    /// <param name="question">User's question</param>
    /// <param name="parkContext">Relevant park data (should be pre-filtered and concise)</param>
    public static string Chat(string question, string parkContext)
    {
        return $@"Answer this question about Ontario Provincial Parks:

Question: {question}

Available park data:
{parkContext}

Provide a helpful, accurate response. If information is unavailable, acknowledge it and share what you can.";
    }

    /// <summary>
    /// Generates a prompt for creating personalized visit itineraries.
    /// Output: JSON with days array and tips array.
    /// </summary>
    /// <param name="parkName">Name of the park</param>
    /// <param name="parkDescription">Brief park description</param>
    /// <param name="activities">Available activities at the park</param>
    /// <param name="durationDays">Number of days for the visit (1-14)</param>
    /// <param name="interests">Visitor's interests</param>
    /// <param name="season">Optional season context</param>
    public static string VisitPlan(string parkName, string parkDescription, List<string> activities, int durationDays, List<string> interests, string? season)
    {
        var interestsList = string.Join(", ", interests);
        var seasonInfo = !string.IsNullOrEmpty(season) ? $"\nSeason: {season}" : "";
        var activitiesList = activities.Any() ? string.Join(", ", activities) : "general outdoor activities";

        return $@"Create a {durationDays}-day itinerary for {parkName}.

Park: {parkDescription}
Activities: {activitiesList}
Interests: {interestsList}{seasonInfo}

JSON format:
{{""days"":[{{""dayNumber"":1,""title"":"""",""activities"":[],""description"":""""}}],""tips"":[]}}

Make it realistic and match visitor interests. Include 2-4 practical tips.";
    }
}
