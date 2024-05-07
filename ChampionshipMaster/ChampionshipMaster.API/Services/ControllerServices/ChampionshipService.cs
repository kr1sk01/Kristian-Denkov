using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipService : ControllerBase, IChampionshipService
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChampionshipDto>> GetAllChampionships()
        {
            var championships = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .ToListAsync();

            var dto = championships.Adapt<List<ChampionshipDto>>();
            return dto;
        }

        public async Task<List<ChampionshipDto>> GetChampionshipsDetails()
        {
            var championshipClassesWithDetails = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .Include(x => x.Games)
                .ThenInclude(g => g.GameStatus)
            .Include(x => x.Games)
                .ThenInclude(g => g.RedTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.BlueTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.Winner).ToListAsync();

            var dtos = championshipClassesWithDetails.Adapt<List<ChampionshipDto>>();
            return dtos;
        }

        public async Task<Championship?> GetChampionship(int id)
        {
            var championshipClass = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .Include(x => x.Games)
                .ThenInclude(g => g.GameStatus)
            .Include(x => x.Games)
                .ThenInclude(g => g.RedTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.BlueTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.Winner)
            .Include(x => x.ChampionshipTeams)
                .ThenInclude(x => x.Team)
            .FirstOrDefaultAsync(a => a.Id == id);

            return championshipClass;
        }

        public async Task<ActionResult<Championship>> PostChampionship(Championship championship)
        {
            if (await ChampionshipNameExists(championship.Name))
            {
                return BadRequest("There is already a championship with that name");
            }

            await _context.Championships.AddAsync(championship);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostChampionship), new
            {
                id = championship.Id
            },
                championship);
        }

        public async Task<bool> ChampionshipNameExists(string? name)
        {
            return await _context.Championships.AnyAsync(x => x.Name!.ToLower() == name!.ToLower());
        }

        public async Task<IActionResult> EditChampionship(int id, Championship championship)
        {
            if (id != championship.Id)
            {
                return BadRequest();
            }

            _context.Entry(championship).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChampionshipExists(id))
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

        public async Task<bool> ChampionshipExists(int id)
        {
            return await _context.Championships.AnyAsync(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteChampionship(int id)
        {
            var championship = await _context.Championships.FindAsync(id);
            if (championship == null)
            {
                return NotFound();
            }

            var championshipTeamsToDelete = await _context.ChampionshipTeams.Where(c => c.ChampionshipId == id).ToListAsync();

            _context.ChampionshipTeams.RemoveRange(championshipTeamsToDelete);
            _context.Championships.Remove(championship);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
