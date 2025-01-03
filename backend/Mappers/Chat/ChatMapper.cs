using llmChat.Dtos.Chat;
using llmChat.Models.Chat;

namespace llmChat.Mappers.Chat
{
    public static class ChatMapper
    {
        public static ChatHistory ToEntity(this ChatCreateDto dto, string userId)
        {
            return new ChatHistory
            {
                CharacterId = dto.CharacterId,
                AppUserId = userId
            };
        }

        public static ChatResponseDto ToDtoWithMessages(this ChatHistory chat)
        {
            return new ChatResponseDto
            {
                ChatId = chat.Id,
                CharacterName = chat.Character.Name,
                Messages = chat.Messages.Select(m => m.ToDto()).ToList()
            };
        }
        public static ChatNameDto ToNameOnlyDto(this ChatHistory chat)
        {
            return new ChatNameDto
            {
                ChatId = chat.Id,
                CharacterName = chat.Character.Name,
            };
        }
    }

}
