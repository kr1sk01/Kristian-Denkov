using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
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

        //public async Task<IActionResult> EditTeam(int id, Team team)
        //{
        //    if (id != team.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(team).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!await TeamExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

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
        public async Task<List<TeamDto>> GetAllActiveTeams()
        {
            var teams = await _context.Teams
                .Include(x => x.TeamType)
                .Include(x => x.TeamPlayers)
                    .ThenInclude(x => x.Player)
                .Where(x => x.Active == true)
                .ToListAsync();

            var dto = teams.Adapt<List<TeamDto>>();

            return dto;
        }
        public async Task<List<TeamDto>> GetAllTeamsPlayerParticipation(string userId)
        {
            var teams = await _context.Teams
                .Include(x => x.TeamType)
                .Where(x => x.CreatedBy == userId)
                .ToListAsync();

            var dto = teams.Adapt<List<TeamDto>>();

            return dto;
        }

        public async Task<ActionResult<TeamDto?>> GetTeam(int id)
        {
            try
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
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return BadRequest();

        }
        public async Task<ActionResult<TeamDto>> SetTeamMembers(string teamId, List<string> playerIds, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var teamToEdit = await _context.Teams.FirstOrDefaultAsync(x => x.Id == int.Parse(teamId));

                if (teamToEdit == null)
                {
                    return NotFound("This team doesn't exist!");
                }

                if (teamToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("Request failed!");
                }

                TeamPlayers teamPlayer;

                //Delete all previous players
                var teamPlayerList = await _context.TeamPlayers.Where(x => x.TeamId == teamToEdit.Id).ToListAsync();
                foreach (var item in teamPlayerList)
                {
                    if (!playerIds.Any(x => x == item.PlayerId))
                        _context.TeamPlayers.Remove(item);
                }

                //Add new ones
                foreach (var playerId in playerIds)
                {
                    if (teamPlayerList.Any(x => x.PlayerId == playerId))
                        continue;

                    var playerToAdd = await _userManager.FindByIdAsync(playerId);

                    teamPlayer = new TeamPlayers
                    {
                        Team = teamToEdit,
                        Player = playerToAdd,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                    };
                    await _context.TeamPlayers.AddAsync(teamPlayer);
                }

                await _context.SaveChangesAsync();
                return Ok("Team members updated successfully!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Couldn't save data to the database!");
            }
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

            var result = new Team
            {
                Name = team.Name,
                TeamType = _context.TeamTypes.FirstOrDefault(x => x.Name == team.TeamTypeName),
                CreatedBy = team.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                Active = true
            };

            await _context.Teams.AddAsync(result);

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

        public async Task<ActionResult> ChangeTeamName(string teamId, string newName, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var teamToEdit = await _context.Teams.FirstOrDefaultAsync(x => x.Id == int.Parse(teamId));

                if (teamToEdit == null)
                {
                    return NotFound("This team doesn't exist!");
                }

                if (teamToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                bool isNewNameExist = await _context.Teams.AnyAsync(x => x.Name == newName && x.Id != teamToEdit.Id);
                if (isNewNameExist)
                {
                    return BadRequest("There is already a team with that name!");
                }

                teamToEdit.Name = newName;
                teamToEdit.ModifiedBy = userId;
                teamToEdit.ModifiedOn = DateTime.UtcNow;
                _context.Entry(teamToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Changed team name successfully!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong");
            }
        }

        public async Task<ActionResult> ChangeTeamLogo(string teamId, string newLogo, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var teamToEdit = await _context.Teams.FirstOrDefaultAsync(x => x.Id == int.Parse(teamId));

                if (teamToEdit == null)
                {
                    return NotFound("This team doesn't exist!");
                }

                if (teamToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                teamToEdit.Logo = Convert.FromBase64String(newLogo);
                teamToEdit.ModifiedBy = userId;
                teamToEdit.ModifiedOn = DateTime.UtcNow;
                _context.Entry(teamToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Changed team logo successfully!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong");
            }
        }

        public async Task<IActionResult> EditTeam(string teamId, TeamDto team, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var teamToEdit = await _context.Teams.FirstOrDefaultAsync(x => x.Id == int.Parse(teamId));

                if (teamToEdit == null)
                {
                    return NotFound("This team doesn't exist!");
                }

                if (teamToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                if (team.Name != teamToEdit.Name && team.Name != null)
                {
                    if (await _context.Teams.AnyAsync(x => x.Name == team.Name))
                    {
                        return BadRequest("There is already a team with that name!");
                    }

                    teamToEdit.Name = team.Name;
                }

                if (team.Logo != null)
                {
                    teamToEdit.Logo = team.Logo;
                }

                if (team.Active != null)
                {
                    teamToEdit.Active = team.Active;
                }

                if (team.Players != null && team.Players.Count > 0)
                {
                    TeamPlayers teamPlayer;
                    var teamPlayerList = await _context.TeamPlayers.Where(x => x.TeamId == teamToEdit.Id).ToListAsync();
                    foreach (var item in teamPlayerList)
                    {
                        if (!team.Players.Any(x => x.Id == item.PlayerId))
                            _context.TeamPlayers.Remove(item);
                    }

                    //Add new ones
                    foreach (var player in team.Players)
                    {
                        if (teamPlayerList.Any(x => x.PlayerId == player.Id))
                            continue;

                        var playerToAdd = await _userManager.FindByIdAsync(player.Id!);

                        teamPlayer = new TeamPlayers
                        {
                            Team = teamToEdit,
                            Player = playerToAdd,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow,
                        };
                        await _context.TeamPlayers.AddAsync(teamPlayer);
                    }
                }

                teamToEdit.ModifiedBy = userId;
                teamToEdit.ModifiedOn = DateTime.UtcNow;

                _context.Entry(teamToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Edited team successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong");
            }
        }
    }
}

