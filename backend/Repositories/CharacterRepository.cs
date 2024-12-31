using llmChat.Data;
using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly ApplicationDBContext _context;

        public CharacterRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Character> CreateAsync(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            return character;
        }

        public async Task<Character?> GetByIdAsync(Guid id)
        {
            return await _context.Characters.Include(c => c.CreatedByAppUser).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Character>> GetByUserIdAsync(string userId)
        {
            return await _context.Characters.Where(c => c.CreatedByAppUserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(Character character)
        {
            _context.Characters.Update(character);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Character character)
        {
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
        }
    }
}
