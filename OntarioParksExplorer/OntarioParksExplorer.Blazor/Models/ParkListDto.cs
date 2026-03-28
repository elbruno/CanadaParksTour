namespace OntarioParksExplorer.Blazor.Models;

public class ParkListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? MainImageUrl { get; set; }
    public List<string> ActivityNames { get; set; } = new();
}
