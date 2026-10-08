namespace DungeonsAndDragqueens.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Services;

[ApiController]
[Route("/[controller]")]
public class StoreMonstersController(
    IStoreMonstersService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StoreMonster>>> Get(
    [FromQuery] bool? outOfStock,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 100,
    [FromQuery] string? sortBy = null)
    {
        try
        {
            var monsters = await service.GetAsync(
                outOfStock,
                page,
                pageSize,
                sortBy);

            return Ok(monsters);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                error = exception.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var monster = await service.GetAsync(id);

        if (monster is null)
        {
            return NotFound(new
            {
                error = "Monster not found."
            });
        }

        return Ok(monster);
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

        if (!deleted)
        {
            return NotFound(new
            {
                error = "Monster not found."
            });
        }

        return NoContent();
    }
}