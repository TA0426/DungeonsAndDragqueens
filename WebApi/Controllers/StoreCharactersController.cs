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
                character.MaxHp,
                character.Damage,
                character.Level,
                character.SwordId,
                character.IsHardcore,
                character.CharacterClass,
                character.BootsId, character.ChestId, character.LegsId, character.GlovesId, character.HelmetId,
                character.SwordLevel, character.BootsLevel, character.ChestLevel, character.LegsLevel, character.GlovesLevel, character.HelmetLevel))
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
            character.MaxHp,
            character.Damage,
            character.Level,
            character.SwordId,
            character.IsHardcore,
            character.CharacterClass,
            character.BootsId, character.ChestId, character.LegsId, character.GlovesId, character.HelmetId,
            character.SwordLevel, character.BootsLevel, character.ChestLevel, character.LegsLevel, character.GlovesLevel, character.HelmetLevel));
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
            character.MaxHp,
            character.Damage,
            character.Level,
            character.SwordId,
            character.IsHardcore,
            character.CharacterClass,
            character.BootsId, character.ChestId, character.LegsId, character.GlovesId, character.HelmetId,
            character.SwordLevel, character.BootsLevel, character.ChestLevel, character.LegsLevel, character.GlovesLevel, character.HelmetLevel);

        return Created(
            $"/StoreCharacters/{character.Id}",
            response);
    }

    [HttpPut("{characterId:guid}/equipment/{swordId:guid}/{level:int}")]
    public async Task<IActionResult> EquipSword(
        Guid characterId,
        Guid swordId,
        int level)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await service.EquipSwordAsync(
            characterId,
            swordId,
            level,
            userId);

        if (character is null)
            return NotFound("Character or sword not found.");

        var sword = await service.GetSwordAsync(character.SwordId!.Value);

        return Ok(new
        {
            swordName = sword?.Name,
            swordDamage = DungeonsAndDragqueens.Core.GearStats.AtLevel(sword?.Damage ?? 0, character.SwordLevel)
        });
    }

    [HttpGet("{id:guid}/inventory")]
    public async Task<IActionResult> GetInventory(Guid id)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await service.GetInventoryAsync(id, userId)); }
        catch (KeyNotFoundException) { return NotFound("Character not found."); }
    }

    [HttpPut("{characterId:guid}/armor/{slot}/{armorId:guid}/{level:int}")]
    public async Task<IActionResult> EquipArmor(Guid characterId, string slot, Guid armorId, int level)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        if (!new[] { "boots", "chest", "legs", "gloves", "helmet" }.Contains(slot.ToLowerInvariant()))
            return BadRequest("Slot must be boots, chest, legs, gloves, or helmet.");
        var character = await service.EquipArmorAsync(characterId, armorId, level, slot, userId);
        if (character is null) return NotFound("Character or matching armor not found.");
        var armor = await service.GetArmorAsync(armorId);
        return Ok(new { armorName = armor!.Name, armor.Slot, defense = DungeonsAndDragqueens.Core.GearStats.AtLevel(armor.Defense, level), swordDamageBonus = DungeonsAndDragqueens.Core.GearStats.AtLevel(armor.SwordDamageBonus, level) });
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
