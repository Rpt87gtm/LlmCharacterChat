namespace llmChat.Interfaces
{
    public interface IMessageGenerator
    {
        Task<string> GenerateResponseAsync(Guid chatId, string userMessage);
    }
}
