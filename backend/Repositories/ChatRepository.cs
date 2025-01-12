using llmChat.Data;
using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ChatRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatHistory> CreateChatAsync(ChatHistory chat)
        {
            _dbContext.ChatHistories.Add(chat);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(chat).Reference(ch => ch.Character).LoadAsync();

            return chat;
        }

        public async Task<ChatHistory?> GetChatByIdAsync(Guid chatId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Character)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task AddMessageAsync(Message message)
        {
            message.SentAt = DateTime.UtcNow;
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateMessageAsync(Message message)
        {
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var chat = await GetChatByIdAsync(chatId);
            if (chat != null)
            {
                _dbContext.ChatHistories.Remove(chat);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Message?> GetMessageByIdAsync(long messageId)
        {
            return await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task DeleteMessageAsync(long messageId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message != null)
            {
                _dbContext.Messages.Remove(message);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ChatHistory?> GetChatWithMessagesAsync(Guid chatId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Messages.OrderBy(m => m.SentAt))
                .Include(ch => ch.Character)
                .FirstOrDefaultAsync(ch => ch.Id == chatId);
        }

        public async Task<List<ChatHistory>> GetChatsByUserIdAsync(string userId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Character)
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
        }
    }

}
