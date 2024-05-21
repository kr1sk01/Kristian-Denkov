using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.API.Services.ControllerServices;
using Microsoft.AspNetCore.Authorization;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // GET: api/Game
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
        {
            var result = await _gameService.GetAllGames();
            return Ok(result);
        }

        // GET: api/Game/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto?>> GetGame(int id)
        {
            var result = await _gameService.GetGame(id);
            return result;
        }

        // PUT: api/Game/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            var result = await _gameService.EditGame(id, game);
            return result;
        }

        // POST: api/Game
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameDto game)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _gameService.PostGame(game, authHeader);
            return result;
        }
        [Authorize]
        [HttpPost("changeGameName")]
        public async Task<IActionResult> ChangeTeamName([FromQuery] string gameId, [FromBody] Dictionary<string, string> content)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            string newName = content.FirstOrDefault().Value;
            var result = await _gameService.ChangeGameName(gameId, newName, authHeader);
            return result;
        }

        // DELETE: api/Game/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var result = await _gameService.DeleteGame(id);
            return result;
        }
    }
}
