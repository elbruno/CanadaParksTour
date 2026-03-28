namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class VisitDay
{
    public int DayNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Activities { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}
