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


}
