using llmChat.Interfaces.Repository;
using llmChat.Interfaces.Services;
using llmChat.Models.Chat;

namespace llmChat.Service
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IChatHistoryRepository _chatHistoryRepository;

        public ChatHistoryService(IChatHistoryRepository chatHistoryRepository)
        {
            _chatHistoryRepository = chatHistoryRepository;
        }

        public async Task<ChatHistory?> GetChatHistoryByIdAsync(Guid chatId)
        {
            return await _chatHistoryRepository.GetChatHistoryByIdAsync(chatId);
        }

        public async Task<List<ChatHistory>> GetChatHistoriesByUserIdAsync(string userId)
        {
            return await _chatHistoryRepository.GetChatHistoriesByUserIdAsync(userId);
        }

        public async Task SaveChatHistoryAsync(ChatHistory chatHistory)
        {
            await _chatHistoryRepository.SaveChatHistoryAsync(chatHistory);
        }
    }
}
