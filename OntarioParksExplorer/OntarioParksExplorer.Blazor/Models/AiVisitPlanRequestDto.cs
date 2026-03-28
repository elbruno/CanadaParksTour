namespace OntarioParksExplorer.Blazor.Models;

public class AiVisitPlanRequestDto
{
    public int ParkId { get; set; }
    public int DurationDays { get; set; }
    public List<string> Interests { get; set; } = new();
    public string? Season { get; set; }
}
