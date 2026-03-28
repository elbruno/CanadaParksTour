namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class VisitPlanRequest
{
    public int ParkId { get; set; }
    public int DurationDays { get; set; }
    public List<string> Interests { get; set; } = new();
    public string? Season { get; set; }
}
