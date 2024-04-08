using GameMaster.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChampionshipMaster.API.Services.Interfaces.ControllerServices
{
    public class GameService : ControllerBase, IGameService
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(id))
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

        public async Task<bool> GameExists(int id)
        {
            return await _context.Games.AnyAsync(x => x.Id == id);
        }

        public async Task<List<GameDto>> GetAllGames()
        {
            var games = await _context.Games
                .Include(x => x.GameType)
                .Include(x => x.GameStatus)
                .Include(x => x.BlueTeam)
                .Include(x => x.RedTeam)
                .Include(x => x.Winner)
                .Include(x => x.Championship)
                .ToListAsync();

            var dto = games.Adapt<List<GameDto>>();
            return dto;
        }

        public async Task<ActionResult<GameDto?>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(x => x.GameType)
                .Include(x => x.GameStatus)
                .Include(x => x.BlueTeam)
                .Include(x => x.RedTeam)
                .Include(x => x.Winner)
                .Include(x => x.Championship)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var dto = game.Adapt<GameDto>();
            return Ok(dto);
        }

        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            _context.Games.Add(game);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await GameExists(game.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            if (game.RedTeamId != null && game.BlueTeamId != null)
            {
                var names = await _context.Teams
                    .Where(x => x.Id == game.RedTeamId || x.Id == game.BlueTeamId)
                    .Select(x => x.Name)
                    .ToListAsync();

                game.Name = $"{names[0]} vs {names[1]}";
            }
            else
            {
                game.Name = $"Game{game.Id}";
            }

            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostGame), new { id = game.Id }, game);
        }
    }
}
