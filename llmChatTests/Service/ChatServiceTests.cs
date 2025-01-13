using llmChat.Models.Chat;
using llmChat.Service.LLMService;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Service.LLMService
{
    public class ChatServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly ChatService _chatService;

        public ChatServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _chatService = new ChatService(_httpClient);
        }

        [Fact]
        public async Task GenerateResponse_ShouldReturnResponse_WhenApiCallIsSuccessful()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
            };

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };

            var expectedResponse = "Hello, how can I help you?";
            var jsonResponse = JsonSerializer.Serialize(new { response = expectedResponse });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _chatService.GenerateResponse(messages, character);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task GenerateResponse_ShouldThrowHttpRequestException_WhenApiCallFails()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
            };

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "Internal Server Error"
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GenerateResponse(messages, character));

            Assert.Equal("Failed to generate response: Internal Server Error", exception.Message);
        }

        [Fact]
        public async Task GenerateResponse_ShouldThrowHttpRequestException_WhenResponseFormatIsInvalid()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
            };

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };

            var jsonResponse = JsonSerializer.Serialize(new { invalid_key = "invalid_value" });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _chatService.GenerateResponse(messages, character));

            Assert.Equal("Invalid response format: 'response' key not found.", exception.Message);
        }
    }
}