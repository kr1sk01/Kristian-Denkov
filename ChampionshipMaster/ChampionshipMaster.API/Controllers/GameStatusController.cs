using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStatusController : ControllerBase
    {
        private readonly IGameStatusService _gameStatusService;

        public GameStatusController(IGameStatusService gameStatusService)
        {
            _gameStatusService = gameStatusService;
        }

        // GET: api/GameStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameStatusDto>>> GetGameStatuses()
        {
            var result = await _gameStatusService.GetAllGameStatuses();
            return Ok(result);
        }

        // GET: api/GameStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameStatus?>> GetGameStatus(int id)
        {
            var result = await _gameStatusService.GetGameStatus(id);
            return result;
        }

        // PUT: api/GameStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameStatus(int id, GameStatus gameStatus)
        {
            var result = await _gameStatusService.EditGameStatus(id, gameStatus);
            return result;
        }

        // POST: api/GameStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameStatus>> PostGameStatus(GameStatus gameStatus)
        {
            var result = await _gameStatusService.PostGameStatus(gameStatus);
            return result;
        }

        // DELETE: api/GameStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameStatus(int id)
        {
            var result = await _gameStatusService.DeleteGameStatus(id);
            return result;
        }
    }
}
