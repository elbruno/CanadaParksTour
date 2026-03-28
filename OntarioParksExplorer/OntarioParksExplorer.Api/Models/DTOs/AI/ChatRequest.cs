namespace OntarioParksExplorer.Api.Models.DTOs.AI;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessage>? ConversationHistory { get; set; }
}
