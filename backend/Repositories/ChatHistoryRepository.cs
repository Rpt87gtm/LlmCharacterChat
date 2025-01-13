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
                .Include(ch => ch.Messages) 
                .FirstOrDefaultAsync(ch => ch.Id == chatHistory.Id);

            if (existingChat == null)
            {
                await _dbContext.ChatHistories.AddAsync(chatHistory);
            }
            else
            {
                _dbContext.Entry(existingChat).CurrentValues.SetValues(chatHistory);

                foreach (var existingMessage in existingChat.Messages.ToList())
                {
                    if (!chatHistory.Messages.Any(m => m.Id == existingMessage.Id))
                    {
                        _dbContext.Messages.Remove(existingMessage);
                    }
                }

                foreach (var message in chatHistory.Messages)
                {
                    message.ChatHistoryId = chatHistory.Id;
                    var existingMessage = existingChat.Messages.FirstOrDefault(m => m.Id == message.Id);
                    if (existingMessage == null)
                    {
                        existingChat.Messages.Add(message);
                    }
                    else
                    {
                        _dbContext.Entry(existingMessage).CurrentValues.SetValues(message);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
