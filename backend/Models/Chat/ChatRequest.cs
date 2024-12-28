using System.Text.Json.Serialization;

namespace llmChat.Models.Chat
{
    public class ChatRequest
    {
        [JsonPropertyName("chatId")]
        public Guid ChatId { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        [JsonPropertyName("streaming")]
        public bool Streaming { get; set; } = false;
    }
}
