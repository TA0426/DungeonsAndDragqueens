namespace OppgaveUkeEnModul3.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.WebApi.Services;

[ApiController]
[Route("/[controller]")]
public class StoreMonstersController(
    IStoreMonstersService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StoreMonster>>> Get()
    {
        var monsters = await service.GetAsync(
            null,
            1,
            100,
            null);

        return Ok(monsters);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var monster = await service.GetAsync(id);

        return monster is null
            ? NotFound()
            : Ok(monster);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateMonsterDTO dto)
    {
        var monster = await service.CreateAsync(dto);

        return Created(
            $"/StoreMonsters/{monster.Id}",
            monster);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);

        return deleted
            ? NoContent()
            : NotFound();
    }
}