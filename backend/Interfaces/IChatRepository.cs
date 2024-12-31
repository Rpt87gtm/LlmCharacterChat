using llmChat.Models.Chat;

namespace llmChat.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatHistory> CreateChatAsync(ChatHistory chat);
        Task<ChatHistory?> GetChatByIdAsync(Guid chatId);
        Task<ChatHistory?> GetChatWithMessagesAsync(Guid chatId);
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteChatAsync(Guid chatId);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task DeleteMessageAsync(Guid messageId);
    }
}
