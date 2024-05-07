using ChampionshipMaster.API.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class TeamPlayersService : ControllerBase, ITeamPlayersService
    {
        private readonly ApplicationDbContext _context;

        public TeamPlayersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteTeamPlayers(int id)
        {
            var teamPlayers = await _context.TeamPlayers.FindAsync(id);
            if (teamPlayers == null)
            {
                return NotFound();
            }

            _context.TeamPlayers.Remove(teamPlayers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditTeamPlayers(int id, TeamPlayers teamPlayers)
        {
            if (id != teamPlayers.Id)
            {
                return BadRequest();
            }

            _context.Entry(teamPlayers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamPlayersExists(id))
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

        public async Task<List<TeamPlayersDto>> GetAllTeamPlayers()
        {
            var teamPlayers = await _context.TeamPlayers.ToListAsync();
            var dto = teamPlayers.Adapt<List<TeamPlayersDto>>();
            return dto;
        }

        public async Task<ActionResult<TeamPlayersDto?>> GetTeamPlayers(int id)
        {
            var teamPlayers = await _context.TeamPlayers.FindAsync(id);

            if (teamPlayers == null)
            {
                return NotFound();
            }

            var dto = teamPlayers.Adapt<TeamPlayersDto>();
            return Ok(dto);
        }

        public async Task<ActionResult<TeamPlayers>> PostTeamPlayers(TeamPlayers teamPlayers)
        {
            await _context.TeamPlayers.AddAsync(teamPlayers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await TeamPlayersExists(teamPlayers.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostTeamPlayers), new { id = teamPlayers.Id }, teamPlayers);
        }

        public async Task<bool> TeamPlayersExists(int id)
        {
            return await _context.TeamPlayers.AnyAsync(x => x.Id == id);
        }
    }
}
