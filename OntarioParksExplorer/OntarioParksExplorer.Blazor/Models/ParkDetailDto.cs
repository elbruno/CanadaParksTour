namespace OntarioParksExplorer.Blazor.Models;

public class ParkDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Website { get; set; }
    public bool IsFeatured { get; set; }
    public string Region { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ActivityDto> Activities { get; set; } = new();
    public List<ParkImageDto> Images { get; set; } = new();
}
