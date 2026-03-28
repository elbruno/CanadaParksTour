namespace OntarioParksExplorer.Api.Models.DTOs;

public class ParkListDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Region { get; set; }
    public bool IsFeatured { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? MainImageUrl { get; set; }
    public List<string> ActivityNames { get; set; } = new();
}
