namespace OppgaveUkeEnModul3.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.WebApi.Services;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("/[controller]")]
public class FightController(GameService gameService) : ControllerBase
{
    private string? GetUserId()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith("Bearer "))
            return null;

        var token = authorization["Bearer ".Length..].Trim();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims
            .FirstOrDefault(claim => claim.Type == "sub")
            ?.Value;
    }
    [HttpPost]
    public async Task<IActionResult> StartFight(StartFightDTO dto)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();


        var result = await gameService.StartFightAsync(
            dto.CharacterId,
            dto.MonsterId,
            userId);

        return Ok(result);
    }
}