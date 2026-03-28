namespace OntarioParksExplorer.Blazor.Models;

public class AiRecommendationDto
{
    public int ParkId { get; set; }
    public string ParkName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
