using llmChat.Dtos.Chat;
using llmChat.Mappers.Chat;
using llmChat.Models.Chat;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Mappers.Chat
{
    public class ChatMapperTests
    {
        [Fact]
        public void ToEntity_ShouldMapChatCreateDtoToChatHistory()
        {
            // Arrange
            var dto = new ChatCreateDto
            {
                CharacterId = Guid.NewGuid()
            };
            var userId = "user1";

            // Act
            var chatHistory = dto.ToEntity(userId);

            // Assert
            Assert.NotNull(chatHistory);
            Assert.Equal(dto.CharacterId, chatHistory.CharacterId);
            Assert.Equal(userId, chatHistory.AppUserId);
        }

        [Fact]
        public void ToEntity_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            ChatCreateDto dto = null;
            var userId = "user1";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => dto.ToEntity(userId));
            Assert.Equal("dto", exception.ParamName);
        }

        [Fact]
        public void ToEntity_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            var dto = new ChatCreateDto
            {
                CharacterId = Guid.NewGuid()
            };
            string userId = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => dto.ToEntity(userId));
            Assert.Equal("userId", exception.ParamName);
        }

        [Fact]
        public void ToDtoWithMessages_ShouldMapChatHistoryToChatResponseDto()
        {
            // Arrange
            var chatHistory = new ChatHistory
            {
                Id = Guid.NewGuid(),
                Character = new Character
                {
                    Name = "Test Character"
                },
                Messages = new List<Message>
                {
                    new Message { Id = 1, Role = "user", Content = "Hello", SentAt = DateTime.UtcNow },
                    new Message { Id = 2, Role = "assistant", Content = "Hi there!", SentAt = DateTime.UtcNow }
                }
            };

            // Act
            var dto = chatHistory.ToDtoWithMessages();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(chatHistory.Id, dto.ChatId);
            Assert.Equal(chatHistory.Character.Name, dto.CharacterName);
            Assert.Equal(2, dto.Messages.Count);
            Assert.Equal("Hello", dto.Messages[0].Content);
            Assert.Equal("Hi there!", dto.Messages[1].Content);
        }

        [Fact]
        public void ToDtoWithMessages_ShouldThrowArgumentNullException_WhenChatHistoryIsNull()
        {
            // Arrange
            ChatHistory chatHistory = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => chatHistory.ToDtoWithMessages());
            Assert.Equal("chat", exception.ParamName);
        }

        [Fact]
        public void ToNameOnlyDto_ShouldMapChatHistoryToChatNameDto()
        {
            // Arrange
            var chatHistory = new ChatHistory
            {
                Id = Guid.NewGuid(),
                Character = new Character
                {
                    Name = "Test Character"
                }
            };

            // Act
            var dto = chatHistory.ToNameOnlyDto();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(chatHistory.Id, dto.ChatId);
            Assert.Equal(chatHistory.Character.Name, dto.CharacterName);
        }

        [Fact]
        public void ToNameOnlyDto_ShouldThrowArgumentNullException_WhenChatHistoryIsNull()
        {
            // Arrange
            ChatHistory chatHistory = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => chatHistory.ToNameOnlyDto());
            Assert.Equal("chat", exception.ParamName);
        }
    }
}