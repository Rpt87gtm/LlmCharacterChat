using llmChat.Data;
using llmChat.Helpers;
using llmChat.Helpers.Pagination;
using llmChat.Interfaces.Repository;
using llmChat.Models.Chat;
using llmChat.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assert = Xunit.Assert;
using api.Models.User;

namespace llmChatTests.Repository
{
    public class CharacterRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDBContext> _options;

        public CharacterRepositoryTests()
        {
            // Используем In-Memory Database для тестов
            _options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для каждой тестовой базы
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCharacterToDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new CharacterRepository(context);
                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Character",
                    SystemPrompt = "Test Prompt", // Обязательное свойство
                    CreatedByAppUserId = "user1"
                };

                // Act
                var result = await repository.CreateAsync(character);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Test Character", result.Name);
                Assert.Equal("Test Prompt", result.SystemPrompt);
                Assert.Equal("user1", result.CreatedByAppUserId);

                // Проверяем, что персонаж действительно добавлен в базу
                var characterFromDb = await context.Characters.FindAsync(character.Id);
                Assert.NotNull(characterFromDb);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCharacterWithUser()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var characterId = Guid.NewGuid();
                var user = new AppUser { Id = "user1", UserName = "testuser" };
                var character = new Character
                {
                    Id = characterId,
                    Name = "Test Character",
                    SystemPrompt = "Test Prompt", // Обязательное свойство
                    CreatedByAppUserId = "user1",
                    CreatedByAppUser = user
                };

                context.Characters.Add(character);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);

                // Act
                var result = await repository.GetByIdAsync(characterId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(characterId, result.Id);
                Assert.NotNull(result.CreatedByAppUser);
                Assert.Equal("testuser", result.CreatedByAppUser.UserName);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_IfCharacterDoesNotExist()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var repository = new CharacterRepository(context);

                // Act
                var result = await repository.GetByIdAsync(Guid.NewGuid()); // Несуществующий ID

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnCharactersForUser()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";
                var character1 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character 1",
                    SystemPrompt = "Prompt 1", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character2 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character 2",
                    SystemPrompt = "Prompt 2", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character3 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character 3",
                    SystemPrompt = "Prompt 3", // Обязательное свойство
                    CreatedByAppUserId = "user2"
                };

                context.Characters.AddRange(character1, character2, character3);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);

                // Act
                var result = await repository.GetByUserIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.All(result, character => Assert.Equal(userId, character.CreatedByAppUserId));
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCharacterInDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var characterId = Guid.NewGuid();
                var character = new Character
                {
                    Id = characterId,
                    Name = "Old Name",
                    SystemPrompt = "Old Prompt", // Обязательное свойство
                    CreatedByAppUserId = "user1"
                };

                context.Characters.Add(character);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);
                character.Name = "Updated Name";
                character.SystemPrompt = "Updated Prompt";

                // Act
                await repository.UpdateAsync(character);

                // Assert
                var updatedCharacter = await context.Characters.FindAsync(characterId);
                Assert.NotNull(updatedCharacter);
                Assert.Equal("Updated Name", updatedCharacter.Name);
                Assert.Equal("Updated Prompt", updatedCharacter.SystemPrompt);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCharacterFromDatabase()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var characterId = Guid.NewGuid();
                var character = new Character
                {
                    Id = characterId,
                    Name = "Test Character",
                    SystemPrompt = "Test Prompt", // Обязательное свойство
                    CreatedByAppUserId = "user1"
                };

                context.Characters.Add(character);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);

                // Act
                await repository.DeleteAsync(character);

                // Assert
                var deletedCharacter = await context.Characters.FindAsync(characterId);
                Assert.Null(deletedCharacter);
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedCharacters()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";

                var user = new AppUser { Id = userId, UserName = "testuser" };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var character1 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character A",
                    SystemPrompt = "Prompt A", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character2 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character B",
                    SystemPrompt = "Prompt B", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character3 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character C",
                    SystemPrompt = "Prompt C", // Обязательное свойство
                    CreatedByAppUserId = "user2"
                };

                context.Characters.AddRange(character1, character2, character3);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);
                var characterQuery = new CharacterQuery { Name = "Character" };
                var queryPage = new QueryPage { PageNumber = 1, PageSize = 2 };

                // Act
                var (characters, totalCount) = await repository.GetAllAsync(characterQuery, queryPage);

                // Assert
                Assert.NotNull(characters);
                Assert.Equal(2, characters.Count);
                Assert.Equal(3, totalCount); // Всего 3 персонажа, но на странице только 2
                Assert.Contains(characters, c => c.Name == "Character A");
                Assert.Contains(characters, c => c.Name == "Character B");
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByName()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";

                
                var user = new AppUser { Id = userId, UserName = "testuser" };
                context.Users.Add(user);
                await context.SaveChangesAsync();

           
                var character1 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character A",
                    SystemPrompt = "Prompt A", 
                    CreatedByAppUserId = userId
                };
                var character2 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character B",
                    SystemPrompt = "Prompt B", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character3 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Other Name",
                    SystemPrompt = "Prompt C", // Обязательное свойство
                    CreatedByAppUserId = userId
                };

                // Добавляем персонажей в контекст и сохраняем
                context.Characters.AddRange(character1, character2, character3);
                await context.SaveChangesAsync();

                // Создаем репозиторий
                var repository = new CharacterRepository(context);

                // Настраиваем запрос
                var characterQuery = new CharacterQuery { Name = "Character" };
                var queryPage = new QueryPage { PageNumber = 1, PageSize = 10 };

                // Act
                var (characters, totalCount) = await repository.GetAllAsync(characterQuery, queryPage);

                // Assert
                Assert.NotNull(characters);
                Assert.Equal(2, characters.Count); // Ожидаем 2 элемента
                Assert.Equal(2, totalCount); // Ожидаем 2 элемента в общей выборке
                Assert.Contains(characters, c => c.Name == "Character A");
                Assert.Contains(characters, c => c.Name == "Character B");
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortByNameDescending()
        {
            // Arrange
            using (var context = new ApplicationDBContext(_options))
            {
                var userId = "user1";

                var user = new AppUser { Id = userId, UserName = "testuser" };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var character1 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character A",
                    SystemPrompt = "Prompt A", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character2 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character B",
                    SystemPrompt = "Prompt B", // Обязательное свойство
                    CreatedByAppUserId = userId
                };
                var character3 = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = "Character C",
                    SystemPrompt = "Prompt C", // Обязательное свойство
                    CreatedByAppUserId = userId
                };

                context.Characters.AddRange(character1, character2, character3);
                await context.SaveChangesAsync();

                var repository = new CharacterRepository(context);
                var characterQuery = new CharacterQuery { SortBy = "Name", IsDescending = true };
                var queryPage = new QueryPage { PageNumber = 1, PageSize = 10 };

                // Act
                var (characters, totalCount) = await repository.GetAllAsync(characterQuery, queryPage);

                // Assert
                Assert.NotNull(characters);
                Assert.Equal(3, characters.Count);
                Assert.Equal("Character C", characters[0].Name);
                Assert.Equal("Character B", characters[1].Name);
                Assert.Equal("Character A", characters[2].Name);
            }
        }
    }
}