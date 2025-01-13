using api.Models.User;
using Azure;
using llmChat.Controllers;
using llmChat.Dtos.Chat;
using llmChat.Helpers;
using llmChat.Helpers.Pagination;
using llmChat.Interfaces.Repository;
using llmChat.Mappers;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Controllers
{
    public class CharacterControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<ICharacterRepository> _mockCharacterRepository;
        private readonly CharacterController _characterController;

        public CharacterControllerTests()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockCharacterRepository = new Mock<ICharacterRepository>();
            _characterController = new CharacterController(_mockCharacterRepository.Object, _mockUserManager.Object);
        }

        private void SetupUser(string username, string userId)
        {
            var user = new AppUser { UserName = username, Id = userId };
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", username)
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _characterController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _mockUserManager
                .Setup(manager => manager.FindByNameAsync(username))
                .ReturnsAsync(user);
        }

        [Fact]
        public async Task CreateCharacter_ShouldReturnCreatedAtAction_WhenCharacterIsCreated()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var createDto = new CharacterCreateDto
            {
                Name = "TestCharacter",
                SystemPrompt = "TestPrompt"
            };

            var character = createDto.ToEntity(userId);
            var createdCharacter = new Character { Id = Guid.NewGuid(), Name = "TestCharacter", SystemPrompt = "TestPrompt", CreatedByAppUserId = userId };

            _mockCharacterRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Character>()))
                .ReturnsAsync(createdCharacter);

            // Act
            var result = await _characterController.CreateCharacter(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CharacterController.GetCharacterById), createdAtActionResult.ActionName);
            Assert.Equal(createdCharacter.Id, ((CharacterDto)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task GetCharacterById_ShouldReturnOk_WhenCharacterExists()
        {
            // Arrange
            var characterId = Guid.NewGuid();
            var character = new Character
            {
                Id = characterId,
                Name = "TestCharacter",
                SystemPrompt = "TestPrompt",
                CreatedByAppUser = new AppUser { UserName = "testuser" } // Инициализируем CreatedByAppUser
            };

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync(character);

            // Act
            var result = await _characterController.GetCharacterById(characterId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as CharacterDto;
            Assert.NotNull(response);
            Assert.Equal(characterId, response.Id);
            Assert.Equal("TestCharacter", response.Name);
            Assert.Equal("TestPrompt", response.SystemPrompt);
            Assert.Equal("testuser", response.CreatedByAppUserName); // Проверяем имя создателя
        }

        [Fact]
        public async Task GetCharacterById_ShouldReturnNotFound_WhenCharacterDoesNotExist()
        {
            // Arrange
            var characterId = Guid.NewGuid();

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync((Character)null);

            // Act
            var result = await _characterController.GetCharacterById(characterId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithPagination()
        {
            // Arrange
            var characters = new List<Character>
            {
                new Character { Id = Guid.NewGuid(), Name = "Character1", SystemPrompt = "Prompt1" },
                new Character { Id = Guid.NewGuid(), Name = "Character2", SystemPrompt = "Prompt2" }
            };

            var characterQuery = new CharacterQuery();
            var queryPage = new QueryPage { PageNumber = 1, PageSize = 10 };

            _mockCharacterRepository
                .Setup(repo => repo.GetAllAsync(characterQuery, queryPage))
                .ReturnsAsync((characters, characters.Count));

            // Act
            var result = await _characterController.GetAll(characterQuery, queryPage);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Assert
            var totalCount = (int)response.GetType().GetProperty("TotalCount").GetValue(response);
            var pageNumber = (int)response.GetType().GetProperty("PageNumber").GetValue(response);
            var pageSize = (int)response.GetType().GetProperty("PageSize").GetValue(response);
            var data = (IEnumerable<CharacterDto>)response.GetType().GetProperty("Data").GetValue(response);

            Assert.NotNull(response);
            Assert.Equal(characters.Count, totalCount);
            Assert.Equal(queryPage.PageNumber, pageNumber);
            Assert.Equal(queryPage.PageSize, pageSize);
            Assert.Equal(2, data.Count());
        }

        [Fact]
        public async Task GetUserCharacters_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var characters = new List<Character>
            {
                new Character { Id = Guid.NewGuid(), Name = "Character1", SystemPrompt = "Prompt1", CreatedByAppUserId = userId },
                new Character { Id = Guid.NewGuid(), Name = "Character2", SystemPrompt = "Prompt2", CreatedByAppUserId = userId }
            };

            _mockCharacterRepository
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(characters);

            // Act
            var result = await _characterController.GetUserCharacters();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as IEnumerable<CharacterDto>;
            Assert.NotNull(response);
            Assert.Equal(characters.Count, response.Count());
        }

        [Fact]
        public async Task GetUserCharacters_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _characterController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = null }
            };

            // Act
            var result = await _characterController.GetUserCharacters();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UpdateCharacter_ShouldReturnOk_WhenCharacterIsUpdated()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var characterId = Guid.NewGuid();
            var character = new Character { Id = characterId, Name = "OldName", SystemPrompt = "OldPrompt", CreatedByAppUserId = userId };

            var updateDto = new CharacterUpdateDto
            {
                Name = "NewName",
                SystemPrompt = "NewPrompt"
            };

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync(character);

            // Act
            var result = await _characterController.UpdateCharacter(characterId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal("NewName", ((CharacterDto)okResult.Value).Name);
            Assert.Equal("NewPrompt", ((CharacterDto)okResult.Value).SystemPrompt);
        }

        [Fact]
        public async Task UpdateCharacter_ShouldReturnNotFound_WhenCharacterDoesNotExist()
        {
            // Arrange
            var characterId = Guid.NewGuid();

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync((Character)null);

            // Act
            var result = await _characterController.UpdateCharacter(characterId, new CharacterUpdateDto());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCharacter_ShouldReturnOk_WhenCharacterIsDeleted()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var characterId = Guid.NewGuid();
            var character = new Character { Id = characterId, Name = "TestCharacter", SystemPrompt = "TestPrompt", CreatedByAppUserId = userId };

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync(character);

            // Act
            var result = await _characterController.DeleteCharacter(characterId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(characterId, ((Character)okResult.Value).Id);
        }

        [Fact]
        public async Task DeleteCharacter_ShouldReturnNotFound_WhenCharacterDoesNotExist()
        {
            // Arrange
            var characterId = Guid.NewGuid();

            _mockCharacterRepository
                .Setup(repo => repo.GetByIdAsync(characterId))
                .ReturnsAsync((Character)null);

            // Act
            var result = await _characterController.DeleteCharacter(characterId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}