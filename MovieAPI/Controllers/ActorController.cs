using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;

namespace MovieAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ActorController : ControllerBase
{
    private readonly ActorService _actorService;

    public ActorController(ActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActors()
    {
        var actors = await _actorService.GetAllActorsAsync();
        return Ok(actors);
    }
}