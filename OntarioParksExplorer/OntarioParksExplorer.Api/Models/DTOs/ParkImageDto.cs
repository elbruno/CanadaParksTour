namespace OntarioParksExplorer.Api.Models.DTOs;

public class ParkImageDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }
}
