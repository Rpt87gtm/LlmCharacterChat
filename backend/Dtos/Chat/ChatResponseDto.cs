﻿namespace llmChat.Dtos.Chat
{
    public class ChatResponseDto
    {
        public Guid ChatId { get; set; }
        public string CharacterName { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new List<ChatMessageDto>();
    }

}
