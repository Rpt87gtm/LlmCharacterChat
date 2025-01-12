using llmChat.Dtos.Chat;
using llmChat.Interfaces.Services;
using llmChat.Interfaces;
using llmChat.Models.Chat;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using llmChat.Mappers.Chat;

public class ChatWebSocketHandler
{
    private readonly IChatRepository _chatRepository;
    private readonly IChatService _chatService;

    public ChatWebSocketHandler(IChatRepository chatRepository, IChatService chatService)
    {
        _chatRepository = chatRepository;
        _chatService = chatService;
    }

    public async Task HandleAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[1024 * 4];

            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            var requestJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            var initRequest = JsonSerializer.Deserialize<ChatInitRequest>(requestJson);

            if (initRequest == null || initRequest.ChatId == Guid.Empty)
            {
                await SendErrorMessageAsync(webSocket, "Invalid initialization request.", cancellationToken);
                return;
            }

            var chat = await _chatRepository.GetChatWithMessagesAsync(initRequest.ChatId);
            if (chat == null)
            {
                await SendErrorMessageAsync(webSocket, "Chat not found.", cancellationToken);
                return;
            }

            var chatInfo = new ChatResponseDto
            {
                ChatId = chat.Id,
                CharacterName = chat.Character.Name,
                Messages = chat.Messages.Select(m => m.ToDto()).ToList()
            };
            await SendResponseAsync(webSocket, chatInfo, cancellationToken);

            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                    break;
                }

                var messageJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var messageDto = JsonSerializer.Deserialize<ChatMessageCreateDto>(messageJson);

                if (messageDto == null)
                {
                    await SendErrorMessageAsync(webSocket, "Invalid message format.", cancellationToken);
                    continue;
                }

                var userMessage = new Message
                {
                    Role = messageDto.Role,
                    Content = messageDto.Content,
                    ChatHistoryId = chatInfo.ChatId,
                };
                await _chatRepository.AddMessageAsync(userMessage);

                var userResponse = new
                {
                    UserMessage = userMessage.ToDto()
                };
                await SendResponseAsync(webSocket, userResponse, cancellationToken);


                Message? assistantMessage = null;
                if (messageDto.Role == "user")
                {
                    var responseContent = await _chatService.GenerateResponse(chat.Messages, chat.Character);
                    assistantMessage = new Message
                    {
                        Role = "assistant",
                        Content = responseContent,
                        ChatHistoryId = chatInfo.ChatId,
                    };
                    await _chatRepository.AddMessageAsync(assistantMessage);
                }

                var assistantResponse = new
                {
                    AssistantMessage = assistantMessage?.ToDto()
                };
                await SendResponseAsync(webSocket, assistantResponse, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Internal server error", CancellationToken.None);
        }
        finally
        {
            webSocket.Dispose();
        }
    }

    private async Task SendResponseAsync(WebSocket webSocket, object response, CancellationToken cancellationToken)
    {
        var responseJson = JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(responseJson);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }

    private async Task SendErrorMessageAsync(WebSocket webSocket, string errorMessage, CancellationToken cancellationToken)
    {
        var errorResponse = new { error = errorMessage };
        var responseJson = JsonSerializer.Serialize(errorResponse);
        var bytes = Encoding.UTF8.GetBytes(responseJson);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }
}

public class ChatInitRequest
{
    public Guid ChatId { get; set; }
}