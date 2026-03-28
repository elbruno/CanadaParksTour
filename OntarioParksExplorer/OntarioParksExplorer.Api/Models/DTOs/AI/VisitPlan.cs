namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class VisitPlan
{
    public string ParkName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public List<VisitDay> Days { get; set; } = new();
    public List<string> Tips { get; set; } = new();
}
