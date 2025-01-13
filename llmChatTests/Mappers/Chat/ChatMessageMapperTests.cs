using llmChat.Dtos.Chat;
using llmChat.Mappers.Chat;
using llmChat.Models.Chat;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Mappers.Chat
{
    public class ChatMessageMapperTests
    {
        [Fact]
        public void ToDto_ShouldMapMessageToChatMessageDto()
        {
            // Arrange
            var message = new Message
            {
                Id = 1,
                Role = "user",
                Content = "Hello, world!",
                ChatHistoryId = Guid.NewGuid(),
                SentAt = DateTime.UtcNow
            };

            // Act
            var dto = message.ToDto();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(message.Id, dto.Id);
            Assert.Equal(message.Role, dto.Role);
            Assert.Equal(message.Content, dto.Content);
            Assert.Equal(message.ChatHistoryId, dto.ChatHistoryId);
            Assert.Equal(message.SentAt, dto.SentAt);
        }

        [Fact]
        public void ToEntity_ShouldMapChatMessageDtoToMessage()
        {
            // Arrange
            var dto = new ChatMessageDto
            {
                Id = 1,
                Role = "assistant",
                Content = "Hi there!",
                ChatHistoryId = Guid.NewGuid(),
                SentAt = DateTime.UtcNow
            };

            // Act
            var message = dto.ToEntity();

            // Assert
            Assert.NotNull(message);
            Assert.Equal(dto.Id, message.Id);
            Assert.Equal(dto.Role, message.Role);
            Assert.Equal(dto.Content, message.Content);
            Assert.Equal(dto.ChatHistoryId, message.ChatHistoryId);
            Assert.Equal(dto.SentAt, message.SentAt);
        }

        [Fact]
        public void ToDto_ShouldHandleNullMessage()
        {
            // Arrange
            Message message = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => message.ToDto());
            Assert.Equal("message", exception.ParamName);
        }

        [Fact]
        public void ToEntity_ShouldHandleNullDto()
        {
            // Arrange
            ChatMessageDto dto = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => dto.ToEntity());
            Assert.Equal("dto", exception.ParamName);
        }
    }
}