using llmChat.Data;
using llmChat.Models.Chat;
using llmChat.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Repository
{
    public class ChatRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDBContext> _options;

        public ChatRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateChatAsync_ShouldAddChatToDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new ChatRepository(context);
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory
                {
                    Id = Guid.NewGuid(),
                    AppUserId = "user1",
                    Character = character
                };

                // Act
                var result = await repository.CreateChatAsync(chat);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(chat.Id, result.Id);
                Assert.Equal("user1", result.AppUserId);
                Assert.NotNull(result.Character);
                Assert.Equal("Test Character", result.Character.Name);

                // Проверяем, что чат действительно добавлен в базу
                var chatFromDb = await context.ChatHistories.FindAsync(chat.Id);
                Assert.NotNull(chatFromDb);
            }
        }

        [Fact]
        public async Task GetChatByIdAsync_ShouldReturnChatWithCharacter()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };

                context.ChatHistories.Add(chat);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                var result = await repository.GetChatByIdAsync(chatId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(chatId, result.Id);
                Assert.NotNull(result.Character);
                Assert.Equal("Test Character", result.Character.Name);
            }
        }

        [Fact]
        public async Task AddMessageAsync_ShouldAddMessageToChat()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                context.ChatHistories.Add(chat);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);
                var message = new Message
                {
                    ChatHistoryId = chatId,
                    Content = "Hello, World!",
                    Role = "user", 
                    SentAt = DateTime.UtcNow 
                };

                // Act
                await repository.AddMessageAsync(message);

                // Assert
                var messageFromDb = await context.Messages.FirstOrDefaultAsync(m => m.ChatHistoryId == chatId);
                Assert.NotNull(messageFromDb);
                Assert.Equal("Hello, World!", messageFromDb.Content);
                Assert.Equal("user", messageFromDb.Role);
                Assert.True(messageFromDb.SentAt <= DateTime.UtcNow);
            }
        }

        [Fact]
        public async Task UpdateMessageAsync_ShouldUpdateMessageContent()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                context.ChatHistories.Add(chat);
                await context.SaveChangesAsync();

                var messageId = 1L;
                var message = new Message
                {
                    Id = messageId,
                    ChatHistoryId = chatId,
                    Content = "Old Content",
                    Role = "user", 
                    SentAt = DateTime.UtcNow 
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);
                message.Content = "Updated Content";

                // Act
                await repository.UpdateMessageAsync(message);

                // Assert
                var updatedMessage = await context.Messages.FindAsync(messageId);
                Assert.NotNull(updatedMessage);
                Assert.Equal("Updated Content", updatedMessage.Content);
                Assert.Equal("user", updatedMessage.Role);
            }
        }

        [Fact]
        public async Task DeleteChatAsync_ShouldRemoveChatFromDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                context.ChatHistories.Add(chat);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                await repository.DeleteChatAsync(chatId);

                // Assert
                var deletedChat = await context.ChatHistories.FindAsync(chatId);
                Assert.Null(deletedChat);
            }
        }

        [Fact]
        public async Task GetChatWithMessagesAsync_ShouldReturnChatWithMessages()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                var message1 = new Message
                {
                    ChatHistoryId = chatId,
                    Content = "Message 1",
                    Role = "user",
                    SentAt = DateTime.UtcNow
                };
                var message2 = new Message
                {
                    ChatHistoryId = chatId,
                    Content = "Message 2",
                    Role = "user",
                    SentAt = DateTime.UtcNow.AddMinutes(1)
                };

                context.ChatHistories.Add(chat);
                context.Messages.AddRange(message1, message2);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                var result = await repository.GetChatWithMessagesAsync(chatId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Messages.Count);
                Assert.Equal("Message 1", result.Messages[0].Content);
                Assert.Equal("Message 2", result.Messages[1].Content);
            }
        }

        [Fact]
        public async Task GetChatsByUserIdAsync_ShouldReturnChatsForUser()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = userId };
                var chat1 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId, Character = character };
                var chat2 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId, Character = character };
                var chat3 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = "user2", Character = character };

                context.ChatHistories.AddRange(chat1, chat2, chat3);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                var result = await repository.GetChatsByUserIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, chat => Assert.Equal(userId, chat.AppUserId));
            }
        }

        [Fact]
        public async Task GetMessageByIdAsync_ShouldReturnMessage()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                context.ChatHistories.Add(chat);

                var messageId = 1L;
                var message = new Message
                {
                    Id = messageId,
                    ChatHistoryId = chatId,
                    Content = "Test Message",
                    Role = "user",
                    SentAt = DateTime.UtcNow
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                var result = await repository.GetMessageByIdAsync(messageId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(messageId, result.Id);
                Assert.Equal("Test Message", result.Content);
                Assert.Equal("user", result.Role);
            }
        }

        [Fact]
        public async Task GetMessageByIdAsync_ShouldReturnNull_IfMessageDoesNotExist()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new ChatRepository(context);

                // Act
                var result = await repository.GetMessageByIdAsync(999); // Несуществующий ID

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task DeleteMessageAsync_ShouldRemoveMessageFromDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var chat = new ChatHistory { Id = chatId, AppUserId = "user1", Character = character };
                context.ChatHistories.Add(chat);

                var messageId = 1L;
                var message = new Message
                {
                    Id = messageId,
                    ChatHistoryId = chatId,
                    Content = "Test Message",
                    Role = "user",
                    SentAt = DateTime.UtcNow
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();

                var repository = new ChatRepository(context);

                // Act
                await repository.DeleteMessageAsync(messageId);

                // Assert
                var deletedMessage = await context.Messages.FindAsync(messageId);
                Assert.Null(deletedMessage);
            }
        }

        [Fact]
        public async Task DeleteMessageAsync_ShouldDoNothing_IfMessageDoesNotExist()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new ChatRepository(context);

                // Act
                await repository.DeleteMessageAsync(999); // Несуществующий ID

                // Assert
                // Ничего не должно произойти, исключений быть не должно
            }
        }
    }
}