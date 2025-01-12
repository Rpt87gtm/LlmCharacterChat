using llmChat.Interfaces.Services;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/model/")]
public class LLMController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IChatService _chatService;
    private readonly IChatHistoryService _chatHistoryService;

    public LLMController(HttpClient httpClient, IChatService chatService, IChatHistoryService chatHistoryService)
    {
        _httpClient = httpClient;
        _chatService = chatService;
        _chatHistoryService = chatHistoryService;
    }


    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (request == null || request.Messages == null || request.Messages.Count == 0)
        {
            return BadRequest("Invalid request. Ensure messages are provided.");
        }

        try
        {
            var chatHistory = await _chatHistoryService.GetChatHistoryByIdAsync(request.ChatId);
            if (chatHistory == null)
            {
                return NotFound($"Chat with ID {request.ChatId} not found.");
            }

            chatHistory.Messages.AddRange(request.Messages);

            var character = chatHistory.Character;

            var responseContent = await _chatService.GenerateResponse(request.Messages, character);

            await _chatHistoryService.SaveChatHistoryAsync(chatHistory);

            return Ok(new
            {
                chatId = chatHistory.Id,
                response = responseContent
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing chat request: {ex.Message}");
        }
    }


    [HttpPost("chatOld")]
    public async Task<IActionResult> ChatOld([FromBody] ChatRequest request)
    {
        if (request == null || request.Messages == null || request.Messages.Count == 0)
        {
            return BadRequest("Invalid request. Ensure messages are provided.");
        }

        try
        {
            var fastApiUrl = "http://localhost:8000/chat";

            var jsonRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(fastApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error communicating with FastAPI service: {ex.Message}");
        }
    }


    [HttpGet("stream")]
    public IActionResult NotSupported()
    {
        return StatusCode(501, "Streaming mode is not supported via HTTP. Use WebSocket directly.");
    }
}
