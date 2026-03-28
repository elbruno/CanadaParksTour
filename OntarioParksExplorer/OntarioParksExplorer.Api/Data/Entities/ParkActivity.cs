namespace OntarioParksExplorer.Api.Data.Entities;

public class ParkActivity
{
    public int ParkId { get; set; }
    public Park Park { get; set; } = null!;

    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
}
