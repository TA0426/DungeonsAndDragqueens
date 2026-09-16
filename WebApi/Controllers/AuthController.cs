using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace OppgaveUkeEnModul3.WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [HttpGet("token")]
    public IActionResult ReadToken()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith("Bearer "))
            return BadRequest("Missing Bearer token.");

        var token = authorization["Bearer ".Length..].Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return Ok(jwt.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            }));
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid JWT token.");
        }
    }
}