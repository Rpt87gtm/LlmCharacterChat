namespace llmChat.Dtos.Chat
{
    public class CharacterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SystemPrompt { get; set; }
        public string CreatedByAppUserId { get; set; }
        public string CreatedByAppUserName { get; set; }
    }
}
