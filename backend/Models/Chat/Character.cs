using api.Models.User;

namespace llmChat.Models.Chat
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SystemPrompt { get; set; }
        public string CreatedByAppUserId { get; set; }
        public AppUser CreatedByAppUser { get; set; }
    }
}
