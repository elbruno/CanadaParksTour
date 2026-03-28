namespace OntarioParksExplorer.Api.Data.Entities;

public class Activity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation properties
    public ICollection<ParkActivity> ParkActivities { get; set; } = new List<ParkActivity>();
}
