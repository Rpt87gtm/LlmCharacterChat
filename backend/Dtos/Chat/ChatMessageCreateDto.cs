namespace llmChat.Dtos.Chat
{
    public class ChatMessageCreateDto
    {
        public Guid ChatId { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
