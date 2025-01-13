using llmChat.Dtos.Chat;
using llmChat.Interfaces.Repository;
using llmChat.Interfaces.Services;
using llmChat.Models.Chat;
using Moq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Xunit;

namespace llmChatTests.WebSocketConf
{
    public class ChatWebSocketHandlerTests
    {
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IChatService> _mockChatService;
        private readonly ChatWebSocketHandler _chatWebSocketHandler;

        public ChatWebSocketHandlerTests()
        {
            _mockChatRepository = new Mock<IChatRepository>();
            _mockChatService = new Mock<IChatService>();
            _chatWebSocketHandler = new ChatWebSocketHandler(_mockChatRepository.Object, _mockChatService.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnChatInfo_WhenChatIsFound()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var chat = new ChatHistory
            {
                Id = chatId,
                Character = new Character { Name = "TestCharacter" },
                Messages = new List<Message>
                {
                    new Message { Role = "user", Content = "Hello", ChatHistoryId = chatId },
                    new Message { Role = "assistant", Content = "Hi there!", ChatHistoryId = chatId }
                }
            };

            var mockWebSocket = new Mock<WebSocket>();
            var buffer = new byte[1024 * 4];
            var initRequest = new ChatInitRequest { ChatId = chatId };
            var requestJson = JsonSerializer.Serialize(initRequest);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);

            _mockChatRepository.Setup(repo => repo.GetChatWithMessagesAsync(chatId))
                .ReturnsAsync(chat);

            mockWebSocket.Setup(ws => ws.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<ArraySegment<byte>, CancellationToken>((data, token) =>
                {
                    requestBytes.AsMemory().CopyTo(data); // Исправлено: явное указание типа Memory<byte>
                })
                .ReturnsAsync(new WebSocketReceiveResult(requestBytes.Length, WebSocketMessageType.Text, true));

            // Act
            await _chatWebSocketHandler.HandleAsync(mockWebSocket.Object, CancellationToken.None);

            // Assert
            _mockChatRepository.Verify(repo => repo.GetChatWithMessagesAsync(chatId), Times.Once);
            mockWebSocket.Verify(ws => ws.SendAsync(
                It.IsAny<ArraySegment<byte>>(),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnError_WhenChatIdIsInvalid()
        {
            // Arrange
            var mockWebSocket = new Mock<WebSocket>();
            var buffer = new byte[1024 * 4];
            var initRequest = new ChatInitRequest { ChatId = Guid.Empty }; // Некорректный ChatId
            var requestJson = JsonSerializer.Serialize(initRequest);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);

            mockWebSocket.Setup(ws => ws.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<ArraySegment<byte>, CancellationToken>((data, token) =>
                {
                    requestBytes.AsMemory().CopyTo(data); // Исправлено: явное указание типа Memory<byte>
                })
                .ReturnsAsync(new WebSocketReceiveResult(requestBytes.Length, WebSocketMessageType.Text, true));

            // Act
            await _chatWebSocketHandler.HandleAsync(mockWebSocket.Object, CancellationToken.None);

            // Assert
            mockWebSocket.Verify(ws => ws.SendAsync(
                It.Is<ArraySegment<byte>>(data => Encoding.UTF8.GetString(data).Contains("Invalid initialization request")),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnError_WhenChatNotFound()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var mockWebSocket = new Mock<WebSocket>();
            var buffer = new byte[1024 * 4];
            var initRequest = new ChatInitRequest { ChatId = chatId };
            var requestJson = JsonSerializer.Serialize(initRequest);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);

            _mockChatRepository.Setup(repo => repo.GetChatWithMessagesAsync(chatId))
                .ReturnsAsync((ChatHistory)null); // Чат не найден

            mockWebSocket.Setup(ws => ws.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Callback<ArraySegment<byte>, CancellationToken>((data, token) =>
                {
                    requestBytes.AsMemory().CopyTo(data); // Исправлено: явное указание типа Memory<byte>
                })
                .ReturnsAsync(new WebSocketReceiveResult(requestBytes.Length, WebSocketMessageType.Text, true));

            // Act
            await _chatWebSocketHandler.HandleAsync(mockWebSocket.Object, CancellationToken.None);

            // Assert
            mockWebSocket.Verify(ws => ws.SendAsync(
                It.Is<ArraySegment<byte>>(data => Encoding.UTF8.GetString(data).Contains("Chat not found")),
                WebSocketMessageType.Text,
                true,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        

    }
}