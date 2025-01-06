using llmChat.Dtos.Chat;
using llmChat.Models.Chat;

namespace llmChat.Mappers
{
    public static class CharacterMapper
    {
        public static CharacterDto ToDtoWithoutCreator(this Character character)
        {
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                SystemPrompt = character.SystemPrompt,
            };
        }

        public static CharacterDto ToDto(this Character character)
        {
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                SystemPrompt = character.SystemPrompt,
                CreatedByAppUserId = character.CreatedByAppUserId,
                CreatedByAppUserName = character.CreatedByAppUser.UserName,
            };
        }

        public static Character ToEntity(this CharacterCreateDto createDto, string userId)
        {
            return new Character
            {
                Name = createDto.Name,
                SystemPrompt = createDto.SystemPrompt,
                CreatedByAppUserId = userId
            };
        }

        public static void UpdateEntity(this Character character, CharacterUpdateDto updateDto)
        {
            character.Name = updateDto.Name;
            character.SystemPrompt = updateDto.SystemPrompt;
        }
    }
}
