namespace OntarioParksExplorer.Api.Data.Entities;

public class ParkImage
{
    public int Id { get; set; }
    public int ParkId { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }

    // Navigation property
    public Park Park { get; set; } = null!;
}
