using llmChat.Interfaces.Services;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Controllers
{
    public class LLMControllerTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IChatHistoryService> _mockChatHistoryService;
        private readonly LLMController _llmController;

        public LLMControllerTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockChatService = new Mock<IChatService>();
            _mockChatHistoryService = new Mock<IChatHistoryService>();
            _llmController = new LLMController(_mockHttpClient.Object, _mockChatService.Object, _mockChatHistoryService.Object);
        }

        [Fact]
        public async Task Chat_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            ChatRequest request = null;

            // Act
            var result = await _llmController.Chat(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request. Ensure messages are provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Chat_ShouldReturnBadRequest_WhenMessagesAreEmpty()
        {
            // Arrange
            var request = new ChatRequest
            {
                ChatId = Guid.NewGuid(),
                Messages = new List<Message>()
            };

            // Act
            var result = await _llmController.Chat(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request. Ensure messages are provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task Chat_ShouldReturnNotFound_WhenChatHistoryDoesNotExist()
        {
            // Arrange
            var request = new ChatRequest
            {
                ChatId = Guid.NewGuid(),
                Messages = new List<Message>
                {
                    new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
                }
            };

            _mockChatHistoryService
                .Setup(service => service.GetChatHistoryByIdAsync(request.ChatId))
                .ReturnsAsync((ChatHistory)null);

            // Act
            var result = await _llmController.Chat(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Chat with ID {request.ChatId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task Chat_ShouldReturnOk_WhenChatHistoryExistsAndResponseIsGenerated()
        {
            // Arrange
            var request = new ChatRequest
            {
                ChatId = Guid.NewGuid(),
                Messages = new List<Message>
        {
            new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
        }
            };

            var chatHistory = new ChatHistory
            {
                Id = request.ChatId,
                Character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Character",
                    SystemPrompt = "You are a helpful assistant."
                },
                Messages = new List<Message>()
            };

            var responseContent = "Hi there!";

            _mockChatHistoryService
                .Setup(service => service.GetChatHistoryByIdAsync(request.ChatId))
                .ReturnsAsync(chatHistory);

            _mockChatService
                .Setup(service => service.GenerateResponse(request.Messages, chatHistory.Character))
                .ReturnsAsync(responseContent);

            _mockChatHistoryService
                .Setup(service => service.SaveChatHistoryAsync(chatHistory))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _llmController.Chat(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Используем рефлексию для проверки свойств анонимного объекта
            var responseType = okResult.Value.GetType();
            var chatIdProperty = responseType.GetProperty("chatId");
            var responseProperty = responseType.GetProperty("response");

            Assert.NotNull(chatIdProperty);
            Assert.NotNull(responseProperty);

            var chatIdValue = chatIdProperty.GetValue(okResult.Value);
            var responseValue = responseProperty.GetValue(okResult.Value);

            Assert.Equal(chatHistory.Id, chatIdValue);
            Assert.Equal(responseContent, responseValue);
        }

        [Fact]
        public async Task Chat_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new ChatRequest
            {
                ChatId = Guid.NewGuid(),
                Messages = new List<Message>
                {
                    new Message { Id = 1, Content = "Hello", Role = "user", SentAt = DateTime.UtcNow }
                }
            };

            _mockChatHistoryService
                .Setup(service => service.GetChatHistoryByIdAsync(request.ChatId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _llmController.Chat(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Error processing chat request: Test exception", statusCodeResult.Value);
        }
    }
}