using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Services;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class StoreSwordsController(
    SwordService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StoreSword>>> Get()
    {
        var swords = await service.GetAsync();

        return Ok(swords);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var sword = await service.GetAsync(id);

        if (sword is null)
        {
            return NotFound(new
            {
                error = "Sword not found."
            });
        }

        return Ok(sword);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(CreateSwordDTO dto)
    {
        var sword = await service.CreateAsync(dto);

        return Created(
            $"/StoreSwords/{sword.Id}",
            sword);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();

    }
}