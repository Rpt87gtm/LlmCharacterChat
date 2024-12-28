using llmChat.Data;
using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Repositories
{
    public class ChatHistoryRepository : IChatHistoryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ChatHistoryRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatHistory?> GetChatHistoryByIdAsync(Guid chatId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Messages)
                .Include(ch => ch.Character)
                .Include(ch => ch.AppUser)
                .FirstOrDefaultAsync(ch => ch.Id == chatId);
        }

        public async Task<List<ChatHistory>> GetChatHistoriesByUserIdAsync(string userId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Character)
                .Where(ch => ch.AppUserId == userId)
                .ToListAsync();
        }

        public async Task SaveChatHistoryAsync(ChatHistory chatHistory)
        {
            var existingChat = await _dbContext.ChatHistories
                .FirstOrDefaultAsync(ch => ch.Id == chatHistory.Id);

            if (existingChat == null)
            {
                await _dbContext.ChatHistories.AddAsync(chatHistory);
            }
            else
            {
                existingChat.Messages = chatHistory.Messages;
                _dbContext.ChatHistories.Update(existingChat);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
