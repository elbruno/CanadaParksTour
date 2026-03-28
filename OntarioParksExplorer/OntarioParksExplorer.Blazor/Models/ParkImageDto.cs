namespace OntarioParksExplorer.Blazor.Models;

public class ParkImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
}
