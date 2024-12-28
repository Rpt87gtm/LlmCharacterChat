using api.Models.User;

namespace llmChat.Models.Chat
{
    public class ChatHistory
    {
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
