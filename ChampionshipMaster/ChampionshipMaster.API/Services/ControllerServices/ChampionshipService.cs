using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipService : ControllerBase, IChampionshipService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILotService _lotService;

        public ChampionshipService(ApplicationDbContext context, ILotService lotService)
        {
            _context = context;
            _lotService = lotService;
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

        public async Task<ChampionshipDto?> GetChampionship(int id)
        {
            var championshipClass = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
                .ThenInclude(x => x.TeamType)
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
                    .ThenInclude(x => x.TeamPlayers)
                        .ThenInclude(x => x.Player)
            .FirstOrDefaultAsync(a => a.Id == id);

            var dto = championshipClass.Adapt<ChampionshipDto>();

            return dto;
        }

        public async Task<ActionResult> PostChampionship(ChampionshipDto championship, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;

                if (userRole.ToLower() != "admin")
                {
                    return Forbid("You do not have permission for this operation");
                }

                if (await _context.Championships.AnyAsync(x => x.Name == championship.Name))
                {
                    return BadRequest("There is already a championship with that name!");
                }

                // Could be refactored to pass the objects instead of looking in the DB
                Championship newChampionship = new Championship()
                {
                    Name = championship.Name,
                    ChampionshipType = await _context.ChampionshipTypes.FirstAsync(x => x.Name == championship.ChampionshipTypeName),
                    ChampionshipStatus = await _context.ChampionshipStatuses.FirstAsync(x => x.Name == "Open"),
                    GameType = await _context.GameTypes.FirstAsync(x => x.Id == championship.GameType!.Id),
                    LotDate = championship.LotDate?.ToUniversalTime(),
                    Date = championship.Date?.ToUniversalTime(),
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                await _context.Championships.AddAsync(newChampionship);
                await _context.SaveChangesAsync();


                return CreatedAtAction(nameof(PostChampionship), new { id = newChampionship.Id });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }

        public async Task<bool> ChampionshipNameExists(string? name)
        {
            return await _context.Championships.AnyAsync(x => x.Name!.ToLower() == name!.ToLower());
        }

        //public async Task<IActionResult> EditChampionship(int id, Championship championship)
        //{
        //    if (id != championship.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(championship).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!await ChampionshipExists(id))
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

        public async Task<IActionResult> JoinChampionship(ChampionshipTeamsDto championshipTeam, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;

                var teamToAdd = await _context.Teams.FirstAsync(x => x.Id == championshipTeam.TeamId);

                if (userId != teamToAdd.CreatedBy && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation");
                }

                var existingChampionshipTeams = await _context.ChampionshipTeams.Where(x => x.ChampionshipId == championshipTeam.ChampionshipId).ToListAsync();

                if (existingChampionshipTeams.Any(x => x.TeamId == teamToAdd.Id))
                {
                    return BadRequest("This team is already registered for this championship");
                }

                ChampionshipTeams championshipTeamToAdd = new()
                {
                    Championship = await _context.Championships.FirstAsync(x => x.Id == championshipTeam.ChampionshipId),
                    Team = teamToAdd,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                };

                await _context.ChampionshipTeams.AddAsync(championshipTeamToAdd);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }

        public async Task<IActionResult> EditChampionship(string championshipId, ChampionshipDto championship, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var championshipToEdit = await _context.Championships.FirstOrDefaultAsync(x => x.Id == int.Parse(championshipId));

                if (championshipToEdit == null)
                {
                    return NotFound("This championship doesn't exist!");
                }

                if (userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                if (championship.Name != championshipToEdit.Name && championship.Name != null)
                {
                    if (await _context.Championships.AnyAsync(x => x.Name == championship.Name))
                    {
                        return BadRequest("There is already a championship with that name!");
                    }

                    championshipToEdit.Name = championship.Name;
                }

                if (championship.Logo != null && championship.Logo.Length != 0)
                {
                    championshipToEdit.Logo = championship.Logo;
                }

                if (championship.ChampionshipStatusName != null && championship.ChampionshipStatusName != championshipToEdit.ChampionshipStatus?.Name)
                {
                    championshipToEdit.ChampionshipStatus = await _context.ChampionshipStatuses.FirstAsync(x => x.Name == championship.ChampionshipStatusName);
                }

                championshipToEdit.LotDate = championship.LotDate;
                championshipToEdit.Date = championship.Date;

                if (championship.WinnerName != null && championship.WinnerName != championshipToEdit.Winner?.Name)
                {
                    championshipToEdit.Winner = await _context.Teams.FirstAsync(x => x.Name == championship.WinnerName);
                    championshipToEdit.ChampionshipStatus = await _context.ChampionshipStatuses.FirstAsync(x => x.Name == "Finished");
                }

                championshipToEdit.ModifiedBy = userId;
                championshipToEdit.ModifiedOn = DateTime.UtcNow;

                _context.Entry(championshipToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Championship edited successfully!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }

        public async Task<IActionResult> DrawLot(int championshipId, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var championshipToEdit = await _context.Championships
                    .Include(x => x.ChampionshipTeams)
                        .ThenInclude(x => x.Team)
                            .ThenInclude(x => x.TeamType)
                        .FirstOrDefaultAsync(x => x.Id == championshipId);

                if (userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                if (championshipToEdit == null)
                {
                    return NotFound("This championship doesn't exist!");
                }

                if (championshipToEdit.LotDate == null)
                {
                    return BadRequest("You cannot draw the lot because the Lot Date is not set!");
                }

                if (DateTime.UtcNow < championshipToEdit.LotDate.Value.ToUniversalTime())
                {
                    return BadRequest("You cannot draw the lot because the Lot Date hasn't arrived yet!");
                }

                if (championshipToEdit.ChampionshipTeams.Count < 2)
                {
                    return BadRequest("You cannot draw the lot when there are less than 2 teams registered!");
                }

                List<Team> teams = [.. championshipToEdit.ChampionshipTeams.Select(x => x.Team)];
                var gameType = await _context.GameTypes.Include(x => x.TeamType).FirstAsync(x => x.TeamType!.Name == teams.First()!.TeamType!.Name);
                var gameStatus = await _context.GameStatuses.FirstAsync(x => x.Name == "Coming");

                int teamsCount = teams.Count;
                int extraGames = _lotService.IsPowerOfTwo(teamsCount) ? 0 : teamsCount - _lotService.LargestPowerOfTwoLessThan(teamsCount);

                _lotService.ShuffleTeams(teams);

                int championshipRounds = _lotService.Rounds(teamsCount);
                int games = teamsCount - 1;

                for (int gameIndex = 1; gameIndex <= games; gameIndex++)
                {
                    int round = _lotService.GetRound(teamsCount, gameIndex);

                    if (round == 1)
                    {
                        await _context.Games.AddAsync(new Game()
                        {
                            Name = $"{teams[0].Name} vs {teams[1].Name}",
                            GameType = gameType,
                            GameStatus = gameStatus,
                            BlueTeam = teams[0],
                            RedTeam = teams[1],
                            Championship = championshipToEdit,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow
                        });

                        teams.RemoveAt(0);
                        teams.RemoveAt(0);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (teams.Count == 0)
                        {
                            for (int i = 0; i < games - gameIndex; i++)
                            {
                                await _context.Games.AddAsync(new Game()
                                {
                                    GameType = gameType,
                                    GameStatus = gameStatus,
                                    Championship = championshipToEdit,
                                    CreatedBy = userId,
                                    CreatedOn = DateTime.UtcNow
                                });

                                await _context.SaveChangesAsync();
                            }

                            break;
                        }
                        else
                        {
                            await _context.Games.AddAsync(new Game()
                            {
                                Name = teams.Count == 2 ? $"{teams[0].Name} vs {teams[1].Name}" : null,
                                GameType = gameType,
                                GameStatus = gameStatus,
                                BlueTeam = teams.Count == 2 ? teams[1] : null,
                                RedTeam = teams[0],
                                Championship = championshipToEdit,
                                CreatedBy = userId,
                                CreatedOn = DateTime.UtcNow
                            });

                            gameIndex++;
                            teams.Clear();
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }
    }
}
