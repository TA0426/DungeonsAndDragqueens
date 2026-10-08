using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Extensions;
using DungeonsAndDragqueens.WebApi.Services;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
[Authorize]
public class StoreCharactersController(CharacterService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StoreCharacter>>> Get()
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var characters = await service.GetAsync(userId);

        var response = characters
            .Select(character => new CharacterResponse(
                character.Id,
                character.Name,
                character.Hp,
                character.Damage,
                character.Level))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await service.GetAsync(id, userId);

        if (character is null)
            return NotFound(new
            {
                error = "Character not found."
            });

        return Ok(new CharacterResponse(
            character.Id,
            character.Name,
            character.Hp,
            character.Damage,
            character.Level));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateCharacterDTO dto)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await service.CreateAsync(dto, userId);

        var response = new CharacterResponse(
            character.Id,
            character.Name,
            character.Hp,
            character.Damage,
            character.Level);

        return Created(
            $"/StoreCharacters/{character.Id}",
            response);
    }

    [HttpPut("{characterId:guid}/equipment/{swordId:guid}")]
    public async Task<IActionResult> EquipSword(
        Guid characterId,
        Guid swordId)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await service.EquipSwordAsync(
            characterId,
            swordId,
            userId);

        if (character is null)
            return NotFound("Character or sword not found.");

        return Ok(character);
    }

    [HttpPost("{id:guid}/camp")]
    public async Task<IActionResult> Camp(Guid id)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await service.CampAsync(id, userId);

        if (result.Character is null)
            return NotFound("Character not found.");

        if (result.RoundsLeft > 0)
        {
            return BadRequest(
                $"You must complete {result.RoundsLeft} more round(s) before camping.");
        }

        return Ok(new
        {
            message =
                $"You rested at camp and restored your HP to {result.Character.MaxHp}.",
            result.Character.Name,
            result.Character.Hp,
            result.Character.Round
        });
    }
}