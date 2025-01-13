using api.Extensions;
using api.Models.User;
using llmChat.Dtos.Chat;
using llmChat.Interfaces.Repository;
using llmChat.Interfaces.Services;
using llmChat.Mappers;
using llmChat.Mappers.Chat;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                if (User == null)
                    return null;

                string username = User.GetUsername();
                return await _userManager.FindByNameAsync(username);

            }
            catch (Exception ex) { 
                return null;
            }
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

            return CreatedAtAction(nameof(GetChatById), new { chatId = createdChat.Id }, createdChat.ToDtoWithMessages());

        }

        [HttpPost("messages")]
        [Authorize]
        public async Task<IActionResult> AddMessage([FromBody] ChatMessageCreateDto messageDto)
        {
            var chat = await _chatRepository.GetChatWithMessagesAsync(messageDto.ChatId);
            if (chat == null)
                return NotFound("Chat not found.");

            var userMessage = new Message
            {
                Role = messageDto.Role,
                Content = messageDto.Content,
                ChatHistoryId = messageDto.ChatId
            };

            await _chatRepository.AddMessageAsync(userMessage);

            Message? assistantMessage = null;


            if (messageDto.Role == "user")
            {
                Console.WriteLine("Start generate");
                var responseContent = await _chatService.GenerateResponse(chat.Messages, chat.Character);
                Console.WriteLine(responseContent);
                assistantMessage = new Message
                {
                    Role = "assistant",
                    Content = responseContent,
                    ChatHistoryId = messageDto.ChatId
                };
                await _chatRepository.AddMessageAsync(assistantMessage);
            }

            return Ok(new
            {
                UserMessage = userMessage.ToDto(),
                AssistantMessage = assistantMessage?.ToDto()
            });
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

            return Ok(chat.ToDtoWithMessages());
        }

        [HttpPut("messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMessage(long messageId, [FromBody] ChatMessageUpdateDto updateDto)
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserChats()
        {
            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            var chats = await _chatRepository.GetChatsByUserIdAsync(appUser.Id);
            Console.WriteLine(chats);
            return Ok(chats.Select(ChatMapper.ToNameOnlyDto));
        }

        [HttpDelete("messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(long messageId)
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
