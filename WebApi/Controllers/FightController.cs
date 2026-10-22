using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Extensions;
using DungeonsAndDragqueens.WebApi.Services;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
[Authorize]
public class FightController(
    GameService gameService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> StartFight(StartFightDTO dto)
    {
        if (dto.CharacterId is null ||
            dto.MonsterId is null ||
            dto.CharacterId == Guid.Empty ||
            dto.MonsterId == Guid.Empty)
        {
            return BadRequest(
                "CharacterId and MonsterId are required.");
        }

        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await gameService.StartFightAsync(
                dto.CharacterId.Value,
                dto.MonsterId.Value,
                userId);

            return Ok(result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    [HttpPost("dungeon")]
    public async Task<IActionResult> StartDungeonFight(StartDungeonFightDTO dto)
    {
        if (dto.CharacterId is null || dto.CharacterId == Guid.Empty || string.IsNullOrWhiteSpace(dto.DungeonId))
            return BadRequest("CharacterId and DungeonId are required.");
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await gameService.StartDungeonFightAsync(dto.CharacterId.Value, dto.DungeonId, userId)); }
        catch (KeyNotFoundException exception) { return NotFound(new { error = exception.Message }); }
        catch (InvalidOperationException exception) { return Conflict(new { error = exception.Message }); }
    }

    [HttpPost("{offerId:guid}/loot")]
    public async Task<IActionResult> ClaimLoot(Guid offerId, ClaimLootDTO dto)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await gameService.ClaimLootAsync(offerId, dto.ChoiceIndex, userId)); }
        catch (InvalidOperationException exception) { return Conflict(new { error = exception.Message }); }
    }
}
