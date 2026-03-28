namespace OntarioParksExplorer.Api.Data.Entities;

public class Park
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

    // Navigation properties
    public ICollection<ParkActivity> ParkActivities { get; set; } = new List<ParkActivity>();
    public ICollection<ParkImage> Images { get; set; } = new List<ParkImage>();
}
