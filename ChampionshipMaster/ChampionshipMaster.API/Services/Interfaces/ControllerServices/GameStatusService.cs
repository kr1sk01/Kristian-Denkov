using ChampionshipMaster.DATA.Models;
using GameMaster.API.Services.Interfaces;

namespace ChampionshipMaster.API.Services.Interfaces.ControllerServices
{
    public class GameStatusService : ControllerBase, IGameStatusService
    {
        private readonly ApplicationDbContext _context;

        public GameStatusService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteGameStatus(int id)
        {
            var gameStatus = await _context.GameStatuses.FindAsync(id);
            if (gameStatus == null)
            {
                return NotFound();
            }

            var games = await _context.Games.Where(x => x.GameStatusId == id).ToListAsync();

            foreach (var game in games)
            {
                game.GameStatusId = null;
                _context.Entry(game).State = EntityState.Modified;
            }

            _context.GameStatuses.Remove(gameStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditGameStatus(int id, GameStatus gameStatus)
        {
            if (id != gameStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        public async Task<bool> GameStatusExists(int id)
        {
            return await _context.GameStatuses.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> GameStatusNameExists(string? name)
        {
            return await _context.GameStatuses.AnyAsync(x => x.Name == name);
        }

        public async Task<List<GameStatus>> GetAllGameStatuses()
        {
            return await _context.GameStatuses.ToListAsync();
        }

        public async Task<ActionResult<GameStatus?>> GetGameStatus(int id)
        {
            var gameStatus = await _context.GameStatuses.FindAsync(id);

            if (gameStatus == null)
            {
                return NotFound();
            }

            return gameStatus;
        }

        public async Task<ActionResult<GameStatus>> PostGameStatus(GameStatus gameStatus)
        {
            if (await GameStatusNameExists(gameStatus.Name))
            {
                return BadRequest("There is already a game status with that name");
            }

            _context.GameStatuses.Add(gameStatus);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await GameStatusExists(gameStatus.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostGameStatus), new { id = gameStatus.Id }, gameStatus);
        }
    }
}
