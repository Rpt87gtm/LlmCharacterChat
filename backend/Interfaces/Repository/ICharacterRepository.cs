using llmChat.Models.Chat;

namespace llmChat.Interfaces.Repository
{
    public interface ICharacterRepository
    {
        Task<Character> CreateAsync(Character character);
        Task<Character?> GetByIdAsync(Guid id);
        Task<List<Character>> GetByUserIdAsync(string userId);
        Task UpdateAsync(Character character);
        Task DeleteAsync(Character character);
    }
}
