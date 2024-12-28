using llmChat.Models.Chat;

namespace llmChat.Interfaces.Services
{
    public interface IChatService
    {
        Task<string> GenerateResponse(List<Message> messages, Character character);
    }
}
