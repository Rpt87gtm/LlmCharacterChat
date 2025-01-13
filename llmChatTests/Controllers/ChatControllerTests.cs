using api.Models.User;
using llmChat.Controllers;
using llmChat.Dtos.Chat;
using llmChat.Interfaces.Repository;
using llmChat.Interfaces.Services;
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
    public class ChatControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IChatService> _mockChatService;
        private readonly ChatController _chatController;

        public ChatControllerTests()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockChatRepository = new Mock<IChatRepository>();
            _mockChatService = new Mock<IChatService>();
            _chatController = new ChatController(_mockChatRepository.Object, _mockUserManager.Object, _mockChatService.Object);
        }

        private void SetupUser(string username, string userId)
        {
                var user = new AppUser { UserName = username, Id = userId }; // Устанавливаем Id
                var claims = new List<Claim>
        {
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", username)
        };
                var identity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(identity);

                _chatController.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = principal }
                };

                _mockUserManager
                    .Setup(manager => manager.FindByNameAsync(username))
                    .ReturnsAsync(user);
        }

        [Fact]
        public async Task CreateChat_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var createDto = new ChatCreateDto { CharacterId = Guid.NewGuid() };

            // Act
            var result = await _chatController.CreateChat(createDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CreateChat_ShouldReturnCreatedAtAction_WhenChatIsCreated()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            // Создаем Character, который будет использоваться в тесте
            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = "charName1",
                SystemPrompt = "SystemPrompt",
                CreatedByAppUserId = userId
            };

            // Создаем DTO для создания чата
            var createDto = new ChatCreateDto { CharacterId = character.Id };

            // Создаем чат, который будет возвращен репозиторием
            var chat = new ChatHistory
            {
                Id = Guid.NewGuid(),
                AppUserId = userId,
                CharacterId = character.Id,
                Character = character // Убедимся, что Character загружен
            };

            // Мок репозитория: возвращаем созданный чат
            _mockChatRepository
                .Setup(repo => repo.CreateChatAsync(It.IsAny<ChatHistory>()))
                .ReturnsAsync(chat);

            // Act
            var result = await _chatController.CreateChat(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ChatController.GetChatById), createdAtActionResult.ActionName);
            Assert.Equal(chat.Id, ((ChatResponseDto)createdAtActionResult.Value).ChatId);
        }

        [Fact]
        public async Task AddMessage_ShouldReturnNotFound_WhenChatDoesNotExist()
        {
            // Arrange
            var messageDto = new ChatMessageCreateDto { ChatId = Guid.NewGuid(), Role = "user", Content = "Hello" };

            _mockChatRepository
                .Setup(repo => repo.GetChatWithMessagesAsync(messageDto.ChatId))
                .ReturnsAsync((ChatHistory)null);

            // Act
            var result = await _chatController.AddMessage(messageDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddMessage_ShouldReturnOk_WhenMessageIsAdded()
        {
            // Arrange
            var messageDto = new ChatMessageCreateDto { ChatId = Guid.NewGuid(), Role = "user", Content = "Hello" };
            var chat = new ChatHistory
            {
                Id = messageDto.ChatId,
                Messages = new List<Message>(),
                Character = new Character { SystemPrompt = "You are a helpful assistant." }
            };

            _mockChatRepository
                .Setup(repo => repo.GetChatWithMessagesAsync(messageDto.ChatId))
                .ReturnsAsync(chat);

            _mockChatService
                .Setup(service => service.GenerateResponse(It.IsAny<List<Message>>(), It.IsAny<Character>()))
                .ReturnsAsync("Hi there!");

            // Act
            var result = await _chatController.AddMessage(messageDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetChatById_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _chatController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = null } // Пользователь не аутентифицирован
            };

            var chatId = Guid.NewGuid();

            // Act
            var result = await _chatController.GetChatById(chatId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result); // Ожидаем Unauthorized
        }

        [Fact]
        public async Task GetChatById_ShouldReturnNotFound_WhenChatDoesNotExist()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var chatId = Guid.NewGuid();

            _mockChatRepository
                .Setup(repo => repo.GetChatWithMessagesAsync(chatId))
                .ReturnsAsync((ChatHistory)null);

            // Act
            var result = await _chatController.GetChatById(chatId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetChatById_ShouldReturnOk_WhenChatExists()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var chatId = Guid.NewGuid();
            var character = new Character { Id = Guid.NewGuid(), Name = "charName1", SystemPrompt = "SystemPrompt" ,CreatedByAppUserId = userId};
            var chat = new ChatHistory { Id = chatId, AppUserId = userId,Character = character }; 

            _mockChatRepository
                .Setup(repo => repo.GetChatWithMessagesAsync(chatId))
                .ReturnsAsync(chat);

            // Act
            var result = await _chatController.GetChatById(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Ожидаем успешный результат
            Assert.NotNull(okResult.Value); // Проверяем, что результат не пустой
        }


        [Fact]
        public async Task UpdateMessage_ShouldReturnNotFound_WhenMessageDoesNotExist()
        {
            // Arrange
            var messageId = 1L;
            var updateDto = new ChatMessageUpdateDto { Content = "Updated content" };

            _mockChatRepository
                .Setup(repo => repo.GetMessageByIdAsync(messageId))
                .ReturnsAsync((Message)null);

            // Act
            var result = await _chatController.UpdateMessage(messageId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateMessage_ShouldReturnOk_WhenMessageIsUpdated()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var messageId = 1L;
            var updateDto = new ChatMessageUpdateDto { Content = "Updated content" };
            var message = new Message { Id = messageId, Content = "Old content", ChatHistoryId = Guid.NewGuid() };

            _mockChatRepository
                .Setup(repo => repo.GetMessageByIdAsync(messageId))
                .ReturnsAsync(message);

            // Act
            var result = await _chatController.UpdateMessage(messageId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task DeleteChat_ShouldReturnNotFound_WhenChatDoesNotExist()
        {
            // Arrange
            var chatId = Guid.NewGuid();

            _mockChatRepository
                .Setup(repo => repo.GetChatByIdAsync(chatId))
                .ReturnsAsync((ChatHistory)null);

            // Act
            var result = await _chatController.DeleteChat(chatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteChat_ShouldReturnNoContent_WhenChatIsDeleted()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var chatId = Guid.NewGuid();
            var chat = new ChatHistory { Id = chatId, AppUserId = "user1" };

            _mockChatRepository
                .Setup(repo => repo.GetChatByIdAsync(chatId))
                .ReturnsAsync(chat);

            // Act
            var result = await _chatController.DeleteChat(chatId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserChats_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _chatController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = null }
            };

            // Act
            var result = await _chatController.GetUserChats();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetUserChats_ShouldReturnOk_WhenChatsExist()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var chats = new List<ChatHistory>
    {
        new ChatHistory { Id = Guid.NewGuid(), AppUserId = "user1" }
    };

            _mockChatRepository
                .Setup(repo => repo.GetChatsByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(chats);

            // Act
            var result = await _chatController.GetUserChats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task DeleteMessage_ShouldReturnNotFound_WhenMessageDoesNotExist()
        {
            // Arrange
            var messageId = 1L;

            _mockChatRepository
                .Setup(repo => repo.GetMessageByIdAsync(messageId))
                .ReturnsAsync((Message)null);

            // Act
            var result = await _chatController.DeleteMessage(messageId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteMessage_ShouldReturnNoContent_WhenMessageIsDeleted()
        {
            // Arrange
            var username = "testuser";
            var userId = "user1";
            SetupUser(username, userId);

            var messageId = 1L;
            var message = new Message { Id = messageId, ChatHistoryId = Guid.NewGuid() };

            _mockChatRepository
                .Setup(repo => repo.GetMessageByIdAsync(messageId))
                .ReturnsAsync(message);

            // Act
            var result = await _chatController.DeleteMessage(messageId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}