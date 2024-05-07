using ChampionshipMaster.API.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipTeamsService : ControllerBase, IChampionshipTeamsService
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipTeamsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChampionshipTeamsExists(int id)
        {
            return await _context.ChampionshipTeams.AnyAsync(t => t.Id == id);
        }

        public async Task<IActionResult> DeleteChampionshipTeams(int id)
        {
            var championshipTeams = await _context.ChampionshipTeams.FindAsync(id);
            if (championshipTeams == null)
            {
                return NotFound();
            }

            _context.ChampionshipTeams.Remove(championshipTeams);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditChampionshipTeams(int id, ChampionshipTeams championshipTeams)
        {
            if (id != championshipTeams.Id)
            {
                return BadRequest();
            }

            _context.Entry(championshipTeams).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChampionshipTeamsExists(id))
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

        public async Task<List<ChampionshipTeamsDto>> GetAllChampionshipTeams()
        {
            var championshipTeams = await _context.ChampionshipTeams.ToListAsync();
            var dto = championshipTeams.Adapt<List<ChampionshipTeamsDto>>();
            return dto;
        }

        public async Task<ActionResult<ChampionshipTeamsDto?>> GetChampionshipTeams(int id)
        {
            var championshipTeams = await _context.ChampionshipTeams.FindAsync(id);

            if (championshipTeams == null)
            {
                return NotFound();
            }

            var dto = championshipTeams.Adapt<ChampionshipDto>();
            return Ok(dto);
        }

        public async Task<ActionResult<ChampionshipTeams>> PostChampionshipTeams(ChampionshipTeams championshipTeams)
        {
            await _context.ChampionshipTeams.AddAsync(championshipTeams);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await ChampionshipTeamsExists(championshipTeams.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostChampionshipTeams), new { id = championshipTeams.Id }, championshipTeams);
        }
    }
}
