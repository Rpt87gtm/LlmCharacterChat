using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using llmChat.Service;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Service
{
    public class ChatHistoryServiceTests
    {
        private readonly Mock<IChatHistoryRepository> _mockChatHistoryRepository;
        private readonly ChatHistoryService _chatHistoryService;

        public ChatHistoryServiceTests()
        {
            _mockChatHistoryRepository = new Mock<IChatHistoryRepository>();
            _chatHistoryService = new ChatHistoryService(_mockChatHistoryRepository.Object);
        }

        [Fact]
        public async Task GetChatHistoryByIdAsync_ShouldReturnChatHistory_WhenChatHistoryExists()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var chatHistory = new ChatHistory
            {
                Id = chatId,
                AppUserId = "user1",
                Messages = new List<Message>
                {
                    new Message { Id = 1, Content = "Message 1", Role = "user", SentAt = DateTime.UtcNow }
                }
            };

            _mockChatHistoryRepository
                .Setup(repo => repo.GetChatHistoryByIdAsync(chatId))
                .ReturnsAsync(chatHistory);

            // Act
            var result = await _chatHistoryService.GetChatHistoryByIdAsync(chatId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(chatId, result.Id);
            Assert.Equal("user1", result.AppUserId);
            Assert.Single(result.Messages);
            Assert.Equal("Message 1", result.Messages[0].Content);
        }

        [Fact]
        public async Task GetChatHistoryByIdAsync_ShouldReturnNull_WhenChatHistoryDoesNotExist()
        {
            // Arrange
            var chatId = Guid.NewGuid();

            _mockChatHistoryRepository
                .Setup(repo => repo.GetChatHistoryByIdAsync(chatId))
                .ReturnsAsync((ChatHistory?)null);

            // Act
            var result = await _chatHistoryService.GetChatHistoryByIdAsync(chatId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetChatHistoriesByUserIdAsync_ShouldReturnChatHistories_WhenUserHasChatHistories()
        {
            // Arrange
            var userId = "user1";
            var chatHistories = new List<ChatHistory>
            {
                new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId },
                new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId }
            };

            _mockChatHistoryRepository
                .Setup(repo => repo.GetChatHistoriesByUserIdAsync(userId))
                .ReturnsAsync(chatHistories);

            // Act
            var result = await _chatHistoryService.GetChatHistoriesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, chat => Assert.Equal(userId, chat.AppUserId));
        }

        [Fact]
        public async Task GetChatHistoriesByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoChatHistories()
        {
            // Arrange
            var userId = "user1";

            _mockChatHistoryRepository
                .Setup(repo => repo.GetChatHistoriesByUserIdAsync(userId))
                .ReturnsAsync(new List<ChatHistory>());

            // Act
            var result = await _chatHistoryService.GetChatHistoriesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SaveChatHistoryAsync_ShouldCallRepositorySaveMethod()
        {
            // Arrange
            var chatHistory = new ChatHistory
            {
                Id = Guid.NewGuid(),
                AppUserId = "user1",
                Messages = new List<Message>
                {
                    new Message { Id = 1, Content = "Message 1", Role = "user", SentAt = DateTime.UtcNow }
                }
            };

            _mockChatHistoryRepository
                .Setup(repo => repo.SaveChatHistoryAsync(chatHistory))
                .Returns(Task.CompletedTask);

            // Act
            await _chatHistoryService.SaveChatHistoryAsync(chatHistory);

            // Assert
            _mockChatHistoryRepository.Verify(repo => repo.SaveChatHistoryAsync(chatHistory), Times.Once);
        }
    }
}