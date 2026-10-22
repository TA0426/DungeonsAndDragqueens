using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DungeonsAndDragqueens.WebApi.Controllers;

[ApiController]
[Route("/[controller]")]
[Authorize]
public class StoreArmorsController(CharacterService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StoreArmor>>> Get() => Ok(await service.GetArmorsAsync());
}
