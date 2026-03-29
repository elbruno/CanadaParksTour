namespace OntarioParksExplorer.Blazor.Models;

public class AppSettingsDto
{
    public string AiProvider { get; set; } = string.Empty;
    public string AiModel { get; set; } = string.Empty;
    public bool AiConfigured { get; set; }
    public string MapProvider { get; set; } = string.Empty;
    public bool MapApiKeyRequired { get; set; }
    public string AppVersion { get; set; } = string.Empty;
}
