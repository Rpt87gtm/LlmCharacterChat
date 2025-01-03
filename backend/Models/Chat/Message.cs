using System.Text.Json.Serialization;

namespace llmChat.Models.Chat
{
    public class Message
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        public Guid ChatHistoryId { get; set; }
        [JsonIgnore]
        public ChatHistory ChatHistory { get; set; }
        public DateTime SentAt { get; set; }
  
    }
}
