namespace OppgaveUkeEnModul3.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using OppgaveUkeEnModul3.Core;
using Microsoft.EntityFrameworkCore;
using OppgaveUkeEnModul3.WebApi.DatabaseContext;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("/[controller]")]


public class StoreCharactersController : ControllerBase
{
    private string? GetUserId()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith("Bearer "))
            return null;

        var token = authorization["Bearer ".Length..].Trim();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims.FirstOrDefault(
            claim => claim.Type == "sub")?.Value;
    }
    private readonly StoreMonstersContext database;

    public StoreCharactersController(StoreMonstersContext database)
    {
        this.database = database;
    }

    [HttpPut("{characterId:guid}/equipment/{swordId:guid}")]
    public async Task<IActionResult> EquipSword(
    Guid characterId,
    Guid swordId)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await database.Characters
            .FirstOrDefaultAsync(character => character.Id == characterId && character.UserId == userId);


        if (character is null)
            return NotFound("Character not found.");

        var sword = await database.Swords
            .FindAsync(swordId);

        if (sword is null)
            return NotFound("Sword not found.");

        character.SwordId = sword.Id;

        await database.SaveChangesAsync();

        return Ok(character);
    }
    [HttpGet]
    public async Task<ActionResult<List<StoreCharacter>>> Get()
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var characters = await database.Characters
            .Where(character => character.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await database.Characters
            .AsNoTracking()
            .FirstOrDefaultAsync(character =>
                character.Id == id &&
                character.UserId == userId);

        return character is null
            ? NotFound()
            : Ok(character);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateCharacterDTO dto)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = new StoreCharacter
        {
            Name = dto.Name,
            UserId = userId
        };

        database.Characters.Add(character);
        await database.SaveChangesAsync();

        return Created(
            $"/StoreCharacters/{character.Id}",
            character);
    }
    [HttpPost("{id:guid}/camp")]
    public async Task<IActionResult> Camp(Guid id)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var character = await database.Characters
            .FirstOrDefaultAsync(character => character.Id == id && character.UserId == userId);


        if (character is null)
            return NotFound("Character not found.");

        var roundsSinceCamp =
            character.Round - character.LastCampRound;

        if (roundsSinceCamp < 3)
        {
            var roundsLeft = 3 - roundsSinceCamp;

            return BadRequest(
                $"You must complete {roundsLeft} more round(s) before camping.");
        }

        character.Hp = character.MaxHp;
        character.LastCampRound = character.Round;

        await database.SaveChangesAsync();

        return Ok(new
        {
            message = $"You rested at camp and restored your HP to {character.MaxHp}.",
            character.Name,
            character.Hp,
            character.Round
        });
    }
}