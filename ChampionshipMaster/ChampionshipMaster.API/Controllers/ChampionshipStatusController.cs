using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipStatusController : ControllerBase
{
    private readonly IChampionshipStatusService _championshipStatusService;

    public ChampionshipStatusController(IChampionshipStatusService championshipStatusService)
    {
        _championshipStatusService = championshipStatusService;
    }


    // GET: api/ChampionshipStatus
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChampionshipStatus>>> Get()
    {
        var result = await _championshipStatusService.GetAllChampionshipStatuses();
        return Ok(result);
    }

    // GET: api/ChampionshipStatus/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipStatus?>> Get(int id)
    {
        var result = await _championshipStatusService.GetChampionshipStatus(id);
        return result;
    }

    // POST: api/ChampionshipStatus
    [HttpPost]
    public async Task<ActionResult<ChampionshipStatus>> Post(ChampionshipStatus championshipStatus)
    {
        var result = await _championshipStatusService.PostChampionshipStatus(championshipStatus);
        return result;
    }

    // PUT: api/ChampionshipStatus/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ChampionshipStatus championshipStatus)
    {
        var result = await _championshipStatusService.EditChampionshipStatus(id, championshipStatus);
        return result;
    }

    // DELETE: api/ChampionshipStatus/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _championshipStatusService.DeleteChampionshipStatus(id);
        return result;
    }
}
