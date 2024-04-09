using ChampionshipMaster.API.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class TeamService : ControllerBase, ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            var championshipTeamsToDelete = await _context.ChampionshipTeams.Where(c => c.TeamId == id).ToListAsync();
            _context.ChampionshipTeams.RemoveRange(championshipTeamsToDelete);

            var teamPlayersToDelete = await _context.TeamPlayers.Where(c => c.TeamId == id).ToListAsync();
            _context.TeamPlayers.RemoveRange(teamPlayersToDelete);

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TeamExists(id))
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

        public async Task<ActionResult<List<GameDto>>> GameHistory(int id)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            var games = await _context.Games
                .Where(x => x.BlueTeamId == id || x.RedTeamId == id)
                    .Include(x => x.GameType)
                    .Include(x => x.GameStatus)
                    .Include(x => x.RedTeam)
                    .Include(x => x.BlueTeam)
                    .Include(x => x.Championship)
                .ToListAsync();

            var dto = games.Adapt<List<GameDto>>();
            return Ok(dto);
        }

        public async Task<List<TeamDto>> GetAllTeams()
        {
            var teams = await _context.Teams
                .Include(x => x.TeamType)
                .ToListAsync();

            var dto = teams.Adapt<List<TeamDto>>();
            return dto;
        }

        public async Task<ActionResult<TeamDto?>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(x => x.TeamType)
                .Include(x => x.TeamPlayers)
                    .ThenInclude(x => x.Player)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            var dto = team.Adapt<TeamDto>();
            return Ok(dto);
        }

        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            if (await TeamNameExists(team.Name))
            {
                return BadRequest("There is already a team with that name");
            }

            _context.Teams.Add(team);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await TeamExists(team.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostTeam), new { id = team.Id }, team);
        }

        public async Task<bool> TeamExists(int id)
        {
            return await _context.Teams.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> TeamNameExists(string? name)
        {
            return await _context.Teams.AnyAsync(x => x.Name == name);
        }
    }
}
