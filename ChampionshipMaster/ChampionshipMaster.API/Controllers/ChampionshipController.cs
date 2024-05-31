using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.SHARED.DTO;
using Microsoft.AspNetCore.Authorization;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipController : ControllerBase
{
    private readonly IChampionshipService _championshipService;

    public ChampionshipController(IChampionshipService championshipService)
    {
        _championshipService = championshipService;
    }

    // GET: api/Championship
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Championship>>> GetChampionships()
    {
        var result = await _championshipService.GetAllChampionships();
        return Ok(result);
    }

    [HttpGet("details")]
    public async Task<ActionResult<IEnumerable<ChampionshipDto>>> GetChampionshipsDetails()
    {
        var result = await _championshipService.GetChampionshipsDetails();
        return Ok(result);
    }

    // GET: api/Championship/5

    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipDto>> GetChampionship(int id)
    {
        var championship = await _championshipService.GetChampionship(id);

        if (championship == null)
        {
            return NotFound();
        }

        var dto = championship.Adapt<ChampionshipDto>();
        return Ok(dto);
    }

    // POST: api/Championship
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> PostChampionship(ChampionshipDto championship)
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        var result = await _championshipService.PostChampionship(championship, authHeader);
        return result;
    }

    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinChampionship(ChampionshipTeamsDto championshipTeams)
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        var result = await _championshipService.JoinChampionship(championshipTeams, authHeader);
        return result;
    }

    // PUT: api/Championship/5
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> PutChampionship([FromQuery] string championshipId, [FromBody] ChampionshipDto championship)
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        var result = await _championshipService.EditChampionship(championshipId, championship, authHeader);
        return result;
    }

    // DELETE: api/Championship/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChampionship(int id)
    {
        var result = await _championshipService.DeleteChampionship(id);
        return result;
    }

    [Authorize]
    [HttpPost("drawLot/{id}")]
    public async Task<IActionResult> DrawLot(int id)
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        var result = await _championshipService.DrawLot(id, authHeader);
        return result;
    }
}
