using api.Models.User;
using llmChat.Dtos.Chat;
using llmChat.Interfaces;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using llmChat.Mappers.Chat;
using api.Extensions;
using llmChat.Interfaces.Services;

namespace llmChat.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;

        public ChatController(
            IChatRepository chatRepository,
            UserManager<AppUser> userManager,
            IChatService chatService)
        {
            _chatRepository = chatRepository;
            _userManager = userManager;
            _chatService = chatService;
        }

        private async Task<AppUser?> GetCurrentUser()
        {
            string username = User.GetUsername();
            return await _userManager.FindByNameAsync(username);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateDto createDto)
        {
            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            var chat = createDto.ToEntity(appUser.Id);
            var createdChat = await _chatRepository.CreateChatAsync(chat);

            return CreatedAtAction(nameof(GetChatById), new { chatId = createdChat.Id }, createdChat.ToDto());

        }

        [HttpPost("messages")]
        [Authorize]
        public async Task<IActionResult> AddMessage([FromBody] ChatMessageCreateDto messageDto)
        {
            var chat = await _chatRepository.GetChatWithMessagesAsync(messageDto.ChatId);
            if (chat == null)
                return NotFound("Chat not found.");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Role = messageDto.Role,
                Content = messageDto.Content,
                ChatHistoryId = messageDto.ChatId
            };

            await _chatRepository.AddMessageAsync(message);

            if (message.Role == "user")
            {
                Console.WriteLine("Start generate");
                var responseContent = await _chatService.GenerateResponse(chat.Messages, chat.Character);
                Console.WriteLine(responseContent);
                var assistantMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Role = "assistant",
                    Content = responseContent,
                    ChatHistoryId = messageDto.ChatId
                };
                await _chatRepository.AddMessageAsync(assistantMessage);
            }

            return Ok(message.ToDto());
        }


        [HttpGet("{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetChatById(Guid chatId)
        {
            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            var chat = await _chatRepository.GetChatWithMessagesAsync(chatId);
            if (chat == null || chat.AppUserId != appUser.Id)
                return NotFound("Chat not found");

            return Ok(chat.ToDto());
        }

        [HttpPut("messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMessage(Guid messageId, [FromBody] ChatMessageUpdateDto updateDto)
        {
            var message = await _chatRepository.GetMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            message.Content = updateDto.Content;
            await _chatRepository.UpdateMessageAsync(message);

            return Ok(message.ToDto());
        }

        [HttpDelete("{chatId}")]
        [Authorize]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
                return NotFound();

            AppUser? appUser = await GetCurrentUser();
            if (appUser == null || chat.AppUserId != appUser.Id)
                return Unauthorized();

            await _chatRepository.DeleteChatAsync(chatId);
            return NoContent();
        }

        [HttpDelete("messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var message = await _chatRepository.GetMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            await _chatRepository.DeleteMessageAsync(messageId);
            return NoContent();
        }
    }
}
