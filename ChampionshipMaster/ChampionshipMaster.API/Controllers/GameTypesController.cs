using ChampionshipMaster.API.Services.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameTypesController : ControllerBase
{
    private readonly IGameTypeService _gameTypeService;

    public GameTypesController(IGameTypeService gameTypeService)
    {
        _gameTypeService = gameTypeService;
    }

    // GET: api/GameTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameTypeDto>>> GetGameTypes()
    {
        var result = await _gameTypeService.GetAllGameTypes();
        return Ok(result);
    }

    // GET: api/GameTypes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameTypeDto?>> GetGameType(int id)
    {
        var result = await _gameTypeService.GetGameType(id);
        return result;
    }

    // PUT: api/GameTypes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGameType(int id, GameType gameType)
    {
        var result = await _gameTypeService.EditGameType(id, gameType);
        return result;
    }

    // POST: api/GameTypes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GameType>> PostGameType(GameType gameType)
    {
        var result = await _gameTypeService.PostGameType(gameType);
        return result;
    }

    // DELETE: api/GameTypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGameType(int id)
    {
        var result = await _gameTypeService.DeleteGameType(id);
        return result;
    }
}
