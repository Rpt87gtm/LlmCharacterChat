using llmChat.Models.Chat;

namespace llmChat.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatHistory> CreateChatAsync(ChatHistory chat);
        Task<ChatHistory?> GetChatByIdAsync(Guid chatId);
        Task<ChatHistory?> GetChatWithMessagesAsync(Guid chatId);
        Task<List<ChatHistory>> GetChatsByUserIdAsync(string userId); 
        Task<Message?> GetMessageByIdAsync(long messageId);
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteChatAsync(Guid chatId);
        Task DeleteMessageAsync(long messageId);
    }
}
