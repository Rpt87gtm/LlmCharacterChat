using llmChat.Models.Chat;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/model/")]
public class LLMController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public LLMController(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    [HttpGet("stream")] //TODO Можно использовать WebSocket-эндпоинт в будущем
    public IActionResult NotSupported()
    {
        return StatusCode(501, "Streaming mode is not supported via HTTP. Use WebSocket directly.");
    }
}