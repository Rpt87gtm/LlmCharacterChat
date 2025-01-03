using llmChat.Data;
using llmChat.Helpers;
using llmChat.Helpers.Pagination;
using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly Paginator _paginator;
        public CharacterRepository(ApplicationDBContext context)
        {
            _context = context;
            _paginator = new Paginator();
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

        public async Task<List<Character>> GetAllAsync(CharacterQuery characterQuery, QueryPage queryPage)
        {
            var characters = _context.Characters.AsQueryable();
            characters = UseQueryParameters(characters, characterQuery);
            characters = _paginator.Paginate(characters, queryPage);

            return await characters.ToListAsync();
        }
        private IQueryable<Character> UseQueryParameters(IQueryable<Character> character, CharacterQuery characterQuery)
        {

            var updatedCharacter = character;

            updatedCharacter = updatedCharacter
                .Where(ch => characterQuery.Name == null || characterQuery.Name == "" || ch.Name.ToLower().Contains(characterQuery.Name.ToLower()));

            if (!String.IsNullOrWhiteSpace(characterQuery.SortBy))
            {
                if (characterQuery.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    updatedCharacter = characterQuery.IdDecsending ? updatedCharacter.OrderByDescending(ch => ch.Name) : updatedCharacter.OrderBy(ch => ch.Name);
                }
            }
            return updatedCharacter;
        }
    }
}
