namespace llmChat.Dtos.Chat
{
    public class ChatMessageDto
    {
        public long Id { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public Guid ChatHistoryId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
