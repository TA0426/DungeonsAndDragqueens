namespace OppgaveUkeEnModul3.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using OppgaveUkeEnModul3.Core;
using Microsoft.EntityFrameworkCore;
using OppgaveUkeEnModul3.WebApi.DatabaseContext;

[ApiController]
[Route("/[controller]")]
public class StoreSwordsController : ControllerBase
{
    private readonly StoreMonstersContext database;

    public StoreSwordsController(StoreMonstersContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<ActionResult<List<StoreSword>>> Get()
    {
        var swords = await database.Swords
            .AsNoTracking()
            .ToListAsync();

        return Ok(swords);
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var sword = database.Swords.FirstOrDefault(
            sword => sword.Id == id);

        return sword is null
            ? NotFound()
            : Ok(sword);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateSwordDTO dto)
    {
        var sword = new StoreSword
        {
            Name = dto.Name,
            Damage = dto.Damage,
            Description = dto.Description
        };

        database.Swords.Add(sword);
        await database.SaveChangesAsync();

        return Created(
            $"/StoreSwords/{sword.Id}",
            sword);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var sword = database.Swords.FirstOrDefault(
            sword => sword.Id == id);

        if (sword is null)
            return NotFound();

        database.Swords.Remove(sword);
        database.SaveChanges();

        return NoContent();
    }
}