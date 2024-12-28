using llmChat.Models.Chat;

namespace llmChat.Interfaces.Repository
{
    public interface IChatHistoryRepository
    {
        Task<ChatHistory?> GetChatHistoryByIdAsync(Guid chatId);
        Task<List<ChatHistory>> GetChatHistoriesByUserIdAsync(string userId);
        Task SaveChatHistoryAsync(ChatHistory chatHistory);
    }
}
