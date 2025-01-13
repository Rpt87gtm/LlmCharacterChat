using llmChat.Dtos.Chat;
using llmChat.Models.Chat;

namespace llmChat.Mappers
{
    public static class CharacterMapper
    {
        public static CharacterDto ToDtoWithoutCreator(this Character character)
        {
            if(character == null) throw new ArgumentNullException(nameof(character));
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                SystemPrompt = character.SystemPrompt,
            };
        }

        public static CharacterDto ToDto(this Character character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                SystemPrompt = character.SystemPrompt,
                CreatedByAppUserId = character.CreatedByAppUserId,
                CreatedByAppUserName = character.CreatedByAppUser?.UserName,
            };
        }

        public static Character ToEntity(this CharacterCreateDto createDto, string userId)
        {
            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException(nameof(userId)); }
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));
            return new Character
            {
                Name = createDto.Name,
                SystemPrompt = createDto.SystemPrompt,
                CreatedByAppUserId = userId
            };
        }

        public static void UpdateEntity(this Character character, CharacterUpdateDto updateDto)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
            character.Name = updateDto.Name;
            character.SystemPrompt = updateDto.SystemPrompt;
        }
    }
}
