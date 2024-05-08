using ChampionshipMaster.API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.Mime.MediaTypeNames;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class TeamService : ControllerBase, ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Player> _userManager;

        public TeamService(ApplicationDbContext context, UserManager<Player> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<List<TeamDto>> GetTeamIparticipate(string username)
        {
            var teams = await _context.Teams
                .Include(x => x.TeamType)
                //.Where(x=>x.CreatedBy.ToUpper() == username.ToUpper())
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
        public async Task<ActionResult<TeamDto>> AddTeamMember(Dictionary<string, string> dict, StringValues authHeader)
        {
            if (await TeamIdExists(int.Parse(dict["teamid"])) == false)
            {
                return BadRequest("This team doesn't exist!");
            }
            var tokenString = authHeader.ToString().Split(' ')[1];
            var token = new JwtSecurityToken(tokenString);

            
            var teamCreatorUsername = token.Claims.First(c => c.Type == "unique_name").Value;

            var teamToAdd = await _context.Teams.FirstOrDefaultAsync(x => x.Id == int.Parse(dict["teamid"]));

            var test = await _userManager.FindByIdAsync(dict["playerId"]);

            TeamPlayers tp = new TeamPlayers
            {
                Team = teamToAdd,               
                Player = test,
                CreatedBy = teamCreatorUsername,
                CreatedOn = DateTime.UtcNow,

            };
            try
            {
                await _context.TeamPlayers.AddAsync(tp);
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                return BadRequest();
            }
            
            
            return Ok();
        }
        public async Task<ActionResult<TeamDto>> PostTeam(TeamDto team, StringValues authHeader)
        {           
            if (await TeamNameExists(team.Name))
            {
                return BadRequest("There is already a team with that name");
            }
            var tokenString = authHeader.ToString().Split(' ')[1];
            var token = new JwtSecurityToken(tokenString);

            var id = token.Claims.First(c => c.Type == "nameid").Value;
            var username = token.Claims.First(c => c.Type == "unique_name").Value;
            
            var result = new Team {
                Name = team.Name,
                TeamType = _context.TeamTypes.FirstOrDefault(x => x.Name == team.TeamTypeName),
                CreatedBy = team.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                Active = true
            };
            ;
            TeamPlayers tp = new TeamPlayers
            {
                Team = result,
                Player = await _userManager.FindByIdAsync(id),
                CreatedBy = username,
                CreatedOn = DateTime.UtcNow,

            };
            await _context.Teams.AddAsync(result);
            await _context.TeamPlayers.AddAsync(tp);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await TeamExists(result.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(PostTeam), new { id = result.Id });
        }

        public async Task<bool> TeamExists(int id)
        {
            return await _context.Teams.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> TeamNameExists(string? name)
        {
            return await _context.Teams.AnyAsync(x => x.Name == name);
        }
        public async Task<bool> TeamIdExists(int? id)
        {
            return await _context.Teams.AnyAsync(x => x.Id == id);
        }
    }
}
