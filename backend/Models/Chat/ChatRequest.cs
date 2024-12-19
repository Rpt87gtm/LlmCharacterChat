using System.Text.Json.Serialization;

namespace llmChat.Models.Chat
{
    public class ChatRequest
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        [JsonPropertyName("streaming")]
        public bool Streaming { get; set; } = false;
    }
}
