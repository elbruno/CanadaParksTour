using Microsoft.Agents.AI;

namespace OntarioParksExplorer.Api.Services.AI;

/// <summary>
/// Provides access to a Copilot-backed AI agent for park-related features.
/// Manages CopilotClient lifecycle and lazy initialization.
/// </summary>
public interface ICopilotAgentProvider
{
    /// <summary>
    /// Gets the AI agent instance. Returns null if Copilot CLI is not available.
    /// </summary>
    Task<AIAgent?> GetAgentAsync();

    /// <summary>
    /// Indicates whether the Copilot agent was successfully initialized.
    /// </summary>
    bool IsConfigured { get; }
}
