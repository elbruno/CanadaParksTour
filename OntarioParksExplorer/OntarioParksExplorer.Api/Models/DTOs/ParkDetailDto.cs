namespace OntarioParksExplorer.Api.Models.DTOs;

public class ParkDetailDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Website { get; set; }
    public bool IsFeatured { get; set; }
    public required string Region { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ActivityDto> Activities { get; set; } = new();
    public List<ParkImageDto> Images { get; set; } = new();
}
