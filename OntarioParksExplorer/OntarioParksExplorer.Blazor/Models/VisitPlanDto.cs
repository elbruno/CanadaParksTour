namespace OntarioParksExplorer.Blazor.Models;

public class VisitPlanDto
{
    public string ParkName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Season { get; set; } = string.Empty;
    public List<DayPlanDto> DayPlans { get; set; } = new();
}

public class DayPlanDto
{
    public int Day { get; set; }
    public string Theme { get; set; } = string.Empty;
    public List<string> Activities { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}
