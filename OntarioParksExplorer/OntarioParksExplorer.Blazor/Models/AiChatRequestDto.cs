namespace OntarioParksExplorer.Blazor.Models;

public class AiChatRequestDto
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessageDto> ConversationHistory { get; set; } = new();
}

public class ChatMessageDto
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
