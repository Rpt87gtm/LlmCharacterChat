using llmChat.Dtos.Chat;
using llmChat.Models.Chat;

namespace llmChat.Mappers.Chat
{
    public static class ChatMessageMapper
    {
        public static ChatMessageDto ToDto(this Message message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                Role = message.Role,
                Content = message.Content,
                ChatHistoryId = message.ChatHistoryId,
                SentAt = message.SentAt,
            };
        }

        public static Message ToEntity(this ChatMessageDto dto)
        {
            return new Message
            {
                Id = dto.Id,
                Role = dto.Role,
                Content = dto.Content,
                ChatHistoryId = dto.ChatHistoryId,
                SentAt = dto.SentAt,
            };
        }
    }
}
