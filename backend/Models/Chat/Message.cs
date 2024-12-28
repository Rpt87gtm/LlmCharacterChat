using System.Text.Json.Serialization;

namespace llmChat.Models.Chat
{
    public class Message
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
