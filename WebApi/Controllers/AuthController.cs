using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DungeonsAndDragqueens.WebApi.Services;
using DungeonsAndDragqueens.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    AuthService authService,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var created = await authService.RegisterAsync(
            request.UserName,
            request.Password);

        if (!created)
        {
            return Conflict(new
            {
                error = "That username is already in use."
            });
        }

        var account = await authService.VerifyCredentialsAsync(
            request.UserName,
            request.Password);

        return Ok(CreateLoginResponse(account!.Id, account.UserName));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var account = await authService.VerifyCredentialsAsync(
            request.UserName,
            request.Password);

        if (account is null)
            return Unauthorized(new { error = "Invalid username or password." });

        return Ok(CreateLoginResponse(account.Id, account.UserName));
    }

    [Authorize]
    [HttpGet("features")]
    public async Task<IActionResult> GetFeatures()
    {
        if (!Guid.TryParse(User.GetUserId(), out var accountId))
            return Unauthorized();

        return Ok(await authService.GetFeaturesAsync(accountId));
    }

    [Authorize]
    [HttpPost("features/{feature}/purchase")]
    public async Task<IActionResult> PurchaseFeature(string feature)
    {
        if (!Guid.TryParse(User.GetUserId(), out var accountId))
            return Unauthorized();

        try
        {
            return Ok(await authService.PurchaseAutoFeatureAsync(accountId, feature));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    private LoginResponse CreateLoginResponse(Guid userId, string userName)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            ],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            userName);
    }
}

public record RegisterRequest(
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    [RegularExpression("^[a-zA-Z0-9_-]+$")]
    string UserName,
    [Required]
    [MinLength(5)]
    [MaxLength(128)]
    string Password);

public record LoginRequest(
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    string UserName,
    [Required]
    [MinLength(1)]
    [MaxLength(128)]
    string Password);

public record LoginResponse(string Token, string UserName);
