using api.Models.User;
using llmChat.Dtos.Chat;
using llmChat.Mappers;
using llmChat.Models.Chat;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Mappers
{
    public class CharacterMapperTests
    {
        [Fact]
        public void ToDtoWithoutCreator_ShouldMapCharacterToCharacterDtoWithoutCreatorInfo()
        {
            // Arrange
            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };

            // Act
            var dto = character.ToDtoWithoutCreator();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(character.Id, dto.Id);
            Assert.Equal(character.Name, dto.Name);
            Assert.Equal(character.SystemPrompt, dto.SystemPrompt);
            Assert.Null(dto.CreatedByAppUserId);
            Assert.Null(dto.CreatedByAppUserName);
        }

        [Fact]
        public void ToDtoWithoutCreator_ShouldThrowArgumentNullException_WhenCharacterIsNull()
        {
            // Arrange
            Character character = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => character.ToDtoWithoutCreator());
            Assert.Equal("character", exception.ParamName);
        }

        [Fact]
        public void ToDto_ShouldMapCharacterToCharacterDtoWithCreatorInfo()
        {
            // Arrange
            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant.",
                CreatedByAppUserId = "user1",
                CreatedByAppUser = new AppUser { UserName = "testuser" }
            };

            // Act
            var dto = character.ToDto();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(character.Id, dto.Id);
            Assert.Equal(character.Name, dto.Name);
            Assert.Equal(character.SystemPrompt, dto.SystemPrompt);
            Assert.Equal(character.CreatedByAppUserId, dto.CreatedByAppUserId);
            Assert.Equal(character.CreatedByAppUser.UserName, dto.CreatedByAppUserName);
        }

        [Fact]
        public void ToDto_ShouldThrowArgumentNullException_WhenCharacterIsNull()
        {
            // Arrange
            Character character = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => character.ToDto());
            Assert.Equal("character", exception.ParamName);
        }

        [Fact]
        public void ToEntity_ShouldMapCharacterCreateDtoToCharacter()
        {
            // Arrange
            var createDto = new CharacterCreateDto
            {
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };
            var userId = "user1";

            // Act
            var character = createDto.ToEntity(userId);

            // Assert
            Assert.NotNull(character);
            Assert.Equal(createDto.Name, character.Name);
            Assert.Equal(createDto.SystemPrompt, character.SystemPrompt);
            Assert.Equal(userId, character.CreatedByAppUserId);
        }

        [Fact]
        public void ToEntity_ShouldThrowArgumentNullException_WhenCreateDtoIsNull()
        {
            // Arrange
            CharacterCreateDto createDto = null;
            var userId = "user1";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => createDto.ToEntity(userId));
            Assert.Equal("createDto", exception.ParamName);
        }

        [Fact]
        public void ToEntity_ShouldThrowArgumentNullException_WhenUserIdIsNull()
        {
            // Arrange
            var createDto = new CharacterCreateDto
            {
                Name = "Test Character",
                SystemPrompt = "You are a helpful assistant."
            };
            string userId = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => createDto.ToEntity(userId));
            Assert.Equal("userId", exception.ParamName);
        }

        [Fact]
        public void UpdateEntity_ShouldUpdateCharacterProperties()
        {
            // Arrange
            var character = new Character
            {
                Name = "Old Name",
                SystemPrompt = "Old Prompt"
            };
            var updateDto = new CharacterUpdateDto
            {
                Name = "New Name",
                SystemPrompt = "New Prompt"
            };

            // Act
            character.UpdateEntity(updateDto);

            // Assert
            Assert.Equal(updateDto.Name, character.Name);
            Assert.Equal(updateDto.SystemPrompt, character.SystemPrompt);
        }

        [Fact]
        public void UpdateEntity_ShouldThrowArgumentNullException_WhenCharacterIsNull()
        {
            // Arrange
            Character character = null;
            var updateDto = new CharacterUpdateDto
            {
                Name = "New Name",
                SystemPrompt = "New Prompt"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => character.UpdateEntity(updateDto));
            Assert.Equal("character", exception.ParamName);
        }

        [Fact]
        public void UpdateEntity_ShouldThrowArgumentNullException_WhenUpdateDtoIsNull()
        {
            // Arrange
            var character = new Character
            {
                Name = "Old Name",
                SystemPrompt = "Old Prompt"
            };
            CharacterUpdateDto updateDto = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => character.UpdateEntity(updateDto));
            Assert.Equal("updateDto", exception.ParamName);
        }
    }
}