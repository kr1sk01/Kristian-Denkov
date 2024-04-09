using ChampionshipMaster.API.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class TeamTypesService : ControllerBase, ITeamTypesService
    {
        private readonly ApplicationDbContext _context;

        public TeamTypesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteTeamType(int id)
        {
            var teamType = await _context.TeamTypes.FindAsync(id);
            if (teamType == null)
            {
                return NotFound();
            }

            var teams = await _context.Teams.Where(x => x.TeamTypeId == id).ToListAsync();
            foreach (var team in teams)
            {
                team.TeamTypeId = null;
                _context.Entry(team).State = EntityState.Modified;
            }

            _context.TeamTypes.Remove(teamType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditTeamType(int id, TeamType teamType)
        {
            if (id != teamType.Id)
            {
                return BadRequest();
            }

            _context.Entry(teamType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamTypeExists(id))
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

        public async Task<List<TeamType>> GetAllTeamTypes()
        {
            return await _context.TeamTypes.ToListAsync();
        }

        public async Task<ActionResult<TeamType?>> GetTeamType(int id)
        {
            var teamType = await _context.TeamTypes.FindAsync(id);

            if (teamType == null)
            {
                return NotFound();
            }

            return Ok(teamType);
        }

        public async Task<ActionResult<TeamType>> PostTeamType(TeamType teamType)
        {
            if (await TeamTypeNameExists(teamType.Name))
            {
                return BadRequest("There is already a team type with that name");
            }

            _context.TeamTypes.Add(teamType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await TeamTypeExists(teamType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostTeamType), new { id = teamType.Id }, teamType);
        }

        public async Task<bool> TeamTypeExists(int id)
        {
            return await _context.TeamTypes.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TeamTypeNameExists(string? name)
        {
            return await _context.TeamTypes.AnyAsync(t => t.Name == name);
        }
    }
}
