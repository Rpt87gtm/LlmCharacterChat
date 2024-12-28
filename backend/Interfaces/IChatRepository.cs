using llmChat.Models.Chat;

namespace llmChat.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatHistory?> GetChatHistoryAsync(Guid chatId);
        Task SaveChatHistoryAsync(ChatHistory chatHistory);
        Task<Character?> GetCharacterAsync(Guid characterId);
        Task<List<Character>> GetCharactersByUserIdAsync(string userId);
        Task SaveCharacterAsync(Character character);
    }
}
