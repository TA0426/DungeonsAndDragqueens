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

        var result = await gameService.StartFightAsync(
            dto.CharacterId.Value,
            dto.MonsterId.Value,
            userId);

        return Ok(result);
    }
}
