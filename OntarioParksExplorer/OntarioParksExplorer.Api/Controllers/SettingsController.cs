using Microsoft.AspNetCore.Mvc;
using OntarioParksExplorer.Api.Services.AI;

namespace OntarioParksExplorer.Api.Controllers;

/// <summary>
/// Provides application configuration and status information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ICopilotAgentProvider _agentProvider;
    private readonly IConfiguration _configuration;

    public SettingsController(ICopilotAgentProvider agentProvider, IConfiguration configuration)
    {
        _agentProvider = agentProvider;
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the current application settings and AI status.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AppSettings>> GetSettings()
    {
        // Trigger lazy initialization to check if Copilot is available
        await _agentProvider.GetAgentAsync();

        return Ok(new AppSettings
        {
            AiProvider = "GitHub Copilot SDK",
            AiModel = _configuration["AI:Model"] ?? "(default)",
            AiConfigured = _agentProvider.IsConfigured,
            MapProvider = "OpenStreetMap",
            MapApiKeyRequired = false,
            AppVersion = "1.0.0"
        });
    }
}

/// <summary>
/// Application settings and status response.
/// </summary>
public class AppSettings
{
    public string AiProvider { get; set; } = string.Empty;
    public string AiModel { get; set; } = string.Empty;
    public bool AiConfigured { get; set; }
    public string MapProvider { get; set; } = string.Empty;
    public bool MapApiKeyRequired { get; set; }
    public string AppVersion { get; set; } = string.Empty;
}
