using llmChat.Data;
using llmChat.Interfaces;
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

        public async Task<ChatHistory?> GetChatHistoryAsync(Guid chatId)
        {
            return await _dbContext.ChatHistories
                .Include(ch => ch.Messages)
                .FirstOrDefaultAsync(ch => ch.Id == chatId);
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
                existingChat.Messages = chatHistory.Messages;
                _dbContext.ChatHistories.Update(existingChat);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Character?> GetCharacterAsync(Guid characterId)
        {
            return await _dbContext.Characters
                .Include(c => c.CreatedByAppUser)
                .FirstOrDefaultAsync(c => c.Id == characterId);
        }

        public async Task<List<Character>> GetCharactersByUserIdAsync(string userId)
        {
            return await _dbContext.Characters
                .Where(c => c.CreatedByAppUserId == userId)
                .ToListAsync();
        }

        public async Task SaveCharacterAsync(Character character)
        {
            var existingCharacter = await _dbContext.Characters
                .FirstOrDefaultAsync(c => c.Id == character.Id);

            if (existingCharacter == null)
            {
                await _dbContext.Characters.AddAsync(character);
            }
            else
            {
                existingCharacter.Name = character.Name;
                existingCharacter.SystemPrompt = character.SystemPrompt;
                _dbContext.Characters.Update(existingCharacter);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
