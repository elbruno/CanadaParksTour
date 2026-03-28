namespace OntarioParksExplorer.Blazor.Models;

public class AiRecommendationRequestDto
{
    public List<string> Activities { get; set; } = new();
    public string? Region { get; set; }
    public string? PreferenceText { get; set; }
}
