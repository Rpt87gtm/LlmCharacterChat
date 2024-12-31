namespace llmChat.Dtos.Chat
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public Guid ChatHistoryId { get; set; }
    }
}
