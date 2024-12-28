using llmChat.Models.Chat;

namespace llmChat.Interfaces.Services
{
    public interface IChatHistoryService
    {
        Task<ChatHistory?> GetChatHistoryByIdAsync(Guid chatId);
        Task<List<ChatHistory>> GetChatHistoriesByUserIdAsync(string userId);
        Task SaveChatHistoryAsync(ChatHistory chatHistory);
    }
}
