using api.Extensions;
using api.Models.User;
using llmChat.Dtos.Chat;
using llmChat.Helpers;
using llmChat.Helpers.Pagination;
using llmChat.Interfaces.Repository;
using llmChat.Mappers;
using llmChat.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace llmChat.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class CharacterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICharacterRepository _characterRepository;

        public CharacterController(ICharacterRepository characterRepository, UserManager<AppUser> userManager)
        {
            _characterRepository = characterRepository;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCharacter([FromBody] CharacterCreateDto createDto)
        {
            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();
            Console.WriteLine(createDto);

            Character character = createDto.ToEntity(appUser.Id);
            Character createdCharacter = await _characterRepository.CreateAsync(character);

            return CreatedAtAction(nameof(GetCharacterById), new { id = createdCharacter.Id }, createdCharacter.ToDto());
        }

        private async Task<AppUser?> GetCurrentUser()
        {
            string username = User.GetUsername();
            return await _userManager.FindByNameAsync(username);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCharacterById(Guid id)
        {
            var character = await _characterRepository.GetByIdAsync(id);

            if (character == null)
                return NotFound();

            return Ok(character.ToDto());
        }

        [HttpGet("GetAllCharacters")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CharacterQuery characterQuery, [FromQuery] QueryPage queryPage)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var (characters, totalCount) = await _characterRepository.GetAllAsync(characterQuery, queryPage);
            return Ok(new
            {
                Data = characters.Select(CharacterMapper.ToDto),
                TotalCount = totalCount,
                PageNumber = queryPage.PageNumber,
                PageSize = queryPage.PageSize,
            });
        }

        [HttpGet("GetUserCharacters")]
        [Authorize]
        public async Task<IActionResult> GetUserCharacters()
        {
            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            var characters = await _characterRepository.GetByUserIdAsync(appUser.Id);
            return Ok(characters.Select(CharacterMapper.ToDtoWithoutCreator));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCharacter(Guid id, [FromBody] CharacterUpdateDto updateDto)
        {
            var character = await _characterRepository.GetByIdAsync(id);

            if (character == null)
                return NotFound("Character not found");

            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            if (character.CreatedByAppUserId != appUser.Id)
                return Forbid();

            character.UpdateEntity(updateDto);
            await _characterRepository.UpdateAsync(character);

            return Ok(character.ToDtoWithoutCreator());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCharacter(Guid id)
        {
            var character = await _characterRepository.GetByIdAsync(id);

            if (character == null)
                return NotFound();

            AppUser? appUser = await GetCurrentUser();
            if (appUser == null)
                return Unauthorized();

            if (character.CreatedByAppUserId != appUser.Id)
                return Forbid();

            await _characterRepository.DeleteAsync(character);
            return Ok(character);
        }
    }
}
