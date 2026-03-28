namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class RecommendationRequest
{
    public List<string> Activities { get; set; } = new();
    public string? Region { get; set; }
    public string? PreferenceText { get; set; }
}
