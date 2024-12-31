using llmChat.Interfaces;
using llmChat.Interfaces.Services;
using llmChat.Models.Chat;
using System.Text;
using System.Text.Json;

namespace llmChat.Service.LLMService
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;

        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateResponse(List<Message> messages, Character character)
        {
            var fastApiUrl = "http://localhost:8000/chat";

            var requestPayload = new
            {
                messages = messages,
                system_prompt = character.SystemPrompt
            };

            var jsonRequest = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(fastApiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                if (responseObject.TryGetProperty("response", out var responseValue))
                {
                    return responseValue.GetString();
                }

                throw new HttpRequestException("Invalid response format: 'response' key not found.");
            }

            throw new HttpRequestException($"Failed to generate response: {response.ReasonPhrase}");
        }
    }
}
