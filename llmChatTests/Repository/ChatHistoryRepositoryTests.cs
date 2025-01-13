using api.Models.User;
using llmChat.Data;
using llmChat.Models.Chat;
using llmChat.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Repository
{
    public class ChatHistoryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDBContext> _options;

        public ChatHistoryRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetChatHistoryByIdAsync_ShouldReturnChatHistoryWithMessagesAndCharacter()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var user = new AppUser { Id = "user1", UserName = "testuser" };
                var chatHistory = new ChatHistory
                {
                    Id = chatId,
                    AppUserId = "user1",
                    Character = character,
                    AppUser = user,
                    Messages = new List<Message>
                    {
                        new Message { Id = 1, Content = "Message 1", Role = "user", SentAt = DateTime.UtcNow },
                        new Message { Id = 2, Content = "Message 2", Role = "assistant", SentAt = DateTime.UtcNow.AddMinutes(1) }
                    }
                };

                context.ChatHistories.Add(chatHistory);
                await context.SaveChangesAsync();

                var repository = new ChatHistoryRepository(context);

                // Act
                var result = await repository.GetChatHistoryByIdAsync(chatId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(chatId, result.Id);
                Assert.NotNull(result.Character);
                Assert.Equal("Test Character", result.Character.Name);
                Assert.NotNull(result.AppUser);
                Assert.Equal("testuser", result.AppUser.UserName);
                Assert.Equal(2, result.Messages.Count);
                Assert.Equal("Message 1", result.Messages[0].Content);
                Assert.Equal("Message 2", result.Messages[1].Content);
            }
        }

        [Fact]
        public async Task GetChatHistoryByIdAsync_ShouldReturnNull_IfChatHistoryDoesNotExist()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new ChatHistoryRepository(context);

                // Act
                var result = await repository.GetChatHistoryByIdAsync(Guid.NewGuid()); // Несуществующий ID

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetChatHistoriesByUserIdAsync_ShouldReturnChatHistoriesForUser()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = userId };
                var chatHistory1 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId, Character = character };
                var chatHistory2 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = userId, Character = character };
                var chatHistory3 = new ChatHistory { Id = Guid.NewGuid(), AppUserId = "user2", Character = character };

                context.ChatHistories.AddRange(chatHistory1, chatHistory2, chatHistory3);
                await context.SaveChangesAsync();

                var repository = new ChatHistoryRepository(context);

                // Act
                var result = await repository.GetChatHistoriesByUserIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, chat => Assert.Equal(userId, chat.AppUserId));
            }
        }

        [Fact]
        public async Task SaveChatHistoryAsync_ShouldAddNewChatHistory()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character { Id = Guid.NewGuid(), Name = "Test Character", SystemPrompt = "Test Prompt", CreatedByAppUserId = "user1" };
                var user = new AppUser { Id = "user1", UserName = "testuser" };
                var chatHistory = new ChatHistory
                {
                    Id = chatId,
                    AppUserId = "user1",
                    Character = character,
                    AppUser = user,
                    Messages = new List<Message>
                    {
                        new Message { Id = 1, Content = "Message 1", Role = "user", SentAt = DateTime.UtcNow }
                    }
                };

                var repository = new ChatHistoryRepository(context);

                // Act
                await repository.SaveChatHistoryAsync(chatHistory);

                // Assert
                var savedChatHistory = await context.ChatHistories
                    .Include(ch => ch.Messages)
                    .Include(ch => ch.Character)
                    .Include(ch => ch.AppUser)
                    .FirstOrDefaultAsync(ch => ch.Id == chatId);

                Assert.NotNull(savedChatHistory);
                Assert.Equal(chatId, savedChatHistory.Id);
                Assert.Equal("user1", savedChatHistory.AppUserId);
                Assert.Equal("Test Character", savedChatHistory.Character.Name);
                Assert.Equal("testuser", savedChatHistory.AppUser.UserName);
                Assert.Single(savedChatHistory.Messages);
                Assert.Equal("Message 1", savedChatHistory.Messages[0].Content);
            }
        }

        [Fact]
        public async Task SaveChatHistoryAsync_ShouldUpdateExistingChatHistory()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var chatId = Guid.NewGuid();
                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Character",
                    SystemPrompt = "Test Prompt",
                    CreatedByAppUserId = "user1"
                };
                var user = new AppUser { Id = "user1", UserName = "testuser" };

                // Добавляем Character и AppUser в базу данных
                context.Characters.Add(character);
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var existingChatHistory = new ChatHistory
                {
                    Id = chatId,
                    AppUserId = "user1",
                    CharacterId = character.Id, // Указываем CharacterId
                    Character = character, // Связываем с существующим Character
                    AppUser = user,
                    Messages = new List<Message>
            {
                new Message { Id = 1, Content = "Old Message", Role = "user", SentAt = DateTime.UtcNow }
            }
                };

                context.ChatHistories.Add(existingChatHistory);
                await context.SaveChangesAsync();

                var updatedChatHistory = new ChatHistory
                {
                    Id = chatId,
                    AppUserId = "user1",
                    CharacterId = character.Id, // Указываем CharacterId
                    Character = character, // Связываем с существующим Character
                    AppUser = user,
                    Messages = new List<Message>
            {
                new Message { Id = 1, Content = "Updated Message", Role = "user", SentAt = DateTime.UtcNow}
            }
                };

                var repository = new ChatHistoryRepository(context);

                // Act
                await repository.SaveChatHistoryAsync(updatedChatHistory);

                // Assert
                var savedChatHistory = await context.ChatHistories
                    .Include(ch => ch.Messages) // Включаем сообщения
                    .Include(ch => ch.Character) // Включаем Character
                    .Include(ch => ch.AppUser) // Включаем AppUser
                    .FirstOrDefaultAsync(ch => ch.Id == chatId);

                Assert.NotNull(savedChatHistory);
                Assert.Equal(chatId, savedChatHistory.Id);
                Assert.Equal("user1", savedChatHistory.AppUserId);
                Assert.NotNull(savedChatHistory.Character); // Проверяем, что Character загружен
                Assert.Equal("Test Character", savedChatHistory.Character.Name);
                Assert.NotNull(savedChatHistory.AppUser); // Проверяем, что AppUser загружен
                Assert.Equal("testuser", savedChatHistory.AppUser.UserName);
                Assert.Single(savedChatHistory.Messages); // Проверяем, что есть одно сообщение
                Assert.Equal("Updated Message", savedChatHistory.Messages[0].Content); // Проверяем содержимое сообщения
            }
        }
    }
}