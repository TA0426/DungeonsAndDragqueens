using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Extensions;
using DungeonsAndDragqueens.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
[Authorize]
public class DungeonsController(GameService gameService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? characterId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        try { return Ok(await gameService.GetDungeonsAsync(characterId, userId)); }
        catch (KeyNotFoundException exception) { return NotFound(new { error = exception.Message }); }
    }
}
