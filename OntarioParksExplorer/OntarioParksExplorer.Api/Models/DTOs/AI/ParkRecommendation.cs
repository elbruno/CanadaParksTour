namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class ParkRecommendation
{
    public int ParkId { get; set; }
    public string ParkName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public double MatchScore { get; set; }
}
