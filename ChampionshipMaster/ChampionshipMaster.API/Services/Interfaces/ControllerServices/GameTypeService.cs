
using ChampionshipMaster.DATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ChampionshipMaster.API.Services.Interfaces.ControllerServices
{
    public class GameTypeService : ControllerBase, IGameTypeService
    {
        private readonly ApplicationDbContext _context;

        public GameTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteGameType(int id)
        {
            var gameType = await _context.GameTypes.FindAsync(id);
            if (gameType == null)
            {
                return NotFound();
            }

            var games = await _context.Games.Where(x => x.GameTypeId == id).ToListAsync();

            foreach (var game in games)
            {
                game.GameTypeId = null;
                _context.Entry(game).State = EntityState.Modified;
            }

            var championships = await _context.Championships.Where(x => x.GameTypeId == id).ToListAsync();

            foreach (var championship in championships)
            {
                championship.GameTypeId = null;
                _context.Entry(championship).State = EntityState.Modified;
            }

            _context.GameTypes.Remove(gameType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditGameType(int id, GameType gameType)
        {
            if (id != gameType.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameTypeExists(id))
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

        public async Task<bool> GameTypeExists(int id)
        {
            return await _context.GameTypes.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> GameTypeNameExists(string? name)
        {
            return await _context.GameTypes.AnyAsync(x => x.Name == name);
        }

        public async Task<List<GameTypeDto>> GetAllGameTypes()
        {
            var gameTypes = await _context.GameTypes
            .Include(x => x.TeamType)
            .ToListAsync();

            var dto = gameTypes.Adapt<List<GameTypeDto>>();
            return dto;
        }

        public async Task<ActionResult<GameTypeDto?>> GetGameType(int id)
        {
            var gameType = await _context.GameTypes
            .Include(x => x.TeamType)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (gameType == null)
            {
                return NotFound();
            }

            var dto = gameType.Adapt<GameTypeDto>();
            return Ok(dto);
        }

        public async Task<ActionResult<GameType>> PostGameType(GameType gameType)
        {
            if (await GameTypeNameExists(gameType.Name))
            {
                return BadRequest("There is already a game type with that name");
            }

            _context.GameTypes.Add(gameType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await GameTypeExists(gameType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostGameType), new { id = gameType.Id }, gameType);
        }
    }
}
