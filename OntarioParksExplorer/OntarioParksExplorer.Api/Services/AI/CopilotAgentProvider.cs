using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;

namespace OntarioParksExplorer.Api.Services.AI;

/// <summary>
/// Singleton provider that manages the CopilotClient lifecycle and exposes an AIAgent
/// backed by GitHub Copilot. Uses lazy initialization with async locking.
/// </summary>
public class CopilotAgentProvider : ICopilotAgentProvider, IAsyncDisposable
{
    private CopilotClient? _copilotClient;
    private AIAgent? _agent;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;
    private bool _isConfigured;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CopilotAgentProvider> _logger;

    private const string DefaultInstructions =
        "You are an AI assistant for Ontario Parks Explorer. " +
        "You help users discover and plan visits to Ontario Provincial Parks. " +
        "Be concise, enthusiastic, and accurate. " +
        "When asked to respond in JSON format, return only valid JSON with no additional text.";

    public CopilotAgentProvider(IConfiguration configuration, ILogger<CopilotAgentProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsConfigured => _isConfigured;

    public async Task<AIAgent?> GetAgentAsync()
    {
        if (_initialized) return _agent;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return _agent;

            _copilotClient = new CopilotClient();
            await _copilotClient.StartAsync();

            var model = _configuration["AI:Model"];
            if (!string.IsNullOrEmpty(model))
            {
                Environment.SetEnvironmentVariable("GITHUB_COPILOT_MODEL", model);
            }

            _agent = _copilotClient.AsAIAgent(
                instructions: DefaultInstructions);

            _isConfigured = true;
            _logger.LogInformation("Copilot AI agent initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to initialize Copilot AI agent. AI features will be disabled. " +
                "Ensure Copilot CLI is installed and authenticated.");
            _isConfigured = false;
            _agent = null;
        }
        finally
        {
            _initialized = true;
            _initLock.Release();
        }

        return _agent;
    }

    public async ValueTask DisposeAsync()
    {
        if (_copilotClient != null)
        {
            await _copilotClient.DisposeAsync();
        }

        _initLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
