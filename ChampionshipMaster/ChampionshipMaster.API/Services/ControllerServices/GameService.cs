﻿using ChampionshipMaster.API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class GameService : ControllerBase, IGameService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Player> _userManager;
        private readonly IEmailSender _emailSender;

        public GameService(ApplicationDbContext context, IEmailSender emailSender, UserManager<Player> userManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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

        public async Task<IActionResult> EditGame(string gameId, GameDto game, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var gameToEdit = await _context.Games
                    .Include(x => x.GameType)
                    .Include(x => x.BlueTeam)
                    .Include(x => x.RedTeam)
                    .FirstOrDefaultAsync(x => x.Id == int.Parse(gameId));

                if (gameToEdit == null)
                {
                    return NotFound("This game doesn't exist!");
                }

                if (gameToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                if (game.Name != gameToEdit.Name && game.Name != null)
                {
                    gameToEdit.Name = game.Name;
                }

                if (game.GameStatusName != null)
                {
                    gameToEdit.GameStatus = await _context.GameStatuses.FirstAsync(x => x.Name == game.GameStatusName);
                }

                if (game.WinnerName != null)
                {
                    gameToEdit.Winner = await _context.Teams.FirstAsync(x => x.Name == game.WinnerName);
                }

                if (game.BluePoints != null)
                {
                    gameToEdit.BluePoints = game.BluePoints;
                    gameToEdit.RedPoints = game.RedPoints;

                    if (game.BluePoints == gameToEdit.GameType!.MaxPoints || game.RedPoints == gameToEdit.GameType!.MaxPoints)
                    {
                        gameToEdit.GameStatus = await _context.GameStatuses.FirstAsync(x => x.Name == "Finished");

                        var teamPlayers = await _context.TeamPlayers
                            .Include(x => x.Player)
                            .Where(x => x.TeamId == gameToEdit.BlueTeamId || x.TeamId == gameToEdit.RedTeamId)
                            .ToListAsync();

                        if (teamPlayers != null)
                        {
                            foreach (var teamPlayer in teamPlayers)
                            {
                                await _emailSender.SendGameFinishedEmail(teamPlayer.Player!.Email!, gameToEdit.Name!, gameToEdit.BlueTeam!.Name!, gameToEdit.BluePoints ?? 0, gameToEdit.RedTeam!.Name!, gameToEdit.RedPoints ?? 0);
                            }
                        }
                        
                    }
                }

                gameToEdit.ModifiedBy = userId;
                gameToEdit.ModifiedOn = DateTime.UtcNow;

                _context.Entry(gameToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }
        public async Task<ActionResult> ChangeGameName(string gameId, string newName, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var userRole = token.Claims.First(x => x.Type == "role").Value;
                var gameToEdit = await _context.Games.FirstOrDefaultAsync(x => x.Id == int.Parse(gameId));

                if (gameToEdit == null)
                {
                    return NotFound("This game doesn't exist!");
                }

                if (gameToEdit.CreatedBy != userId && userRole != "admin")
                {
                    return Forbid("You do not have permission for this operation!");
                }

                bool isNewNameExist = await _context.Games.AnyAsync(x => x.Name == newName && x.Id != gameToEdit.Id);
                if (isNewNameExist)
                {
                    return BadRequest("There is already a game with that name!");
                }

                gameToEdit.Name = newName;
                gameToEdit.ModifiedBy = userId;
                gameToEdit.ModifiedOn = DateTime.UtcNow;
                _context.Entry(gameToEdit).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Changed game name successfully!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
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

        public async Task<ActionResult> PostGame(GameDto game, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                var blueTeam = await _context.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).FirstAsync(x => x.Name == game.BlueTeam!.Name);
                var redTeam = await _context.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).FirstAsync(x => x.Name == game.RedTeam!.Name);

                if (blueTeam.TeamPlayers.Any(x => redTeam.TeamPlayers.Any(y => y.PlayerId == x.PlayerId)))
                {
                    return BadRequest("The teams you've selected contain 1 or more repeating players!");
                }

                Game newGame = new Game()
                {
                    Name = game.Name,
                    GameType = await _context.GameTypes.FirstAsync(x => x.Name == game.GameTypeName),
                    BlueTeam = blueTeam,
                    RedTeam = redTeam,
                    Date = game.Date!.Value.ToUniversalTime(),
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                await _context.Games.AddAsync(newGame);
                await _context.SaveChangesAsync();

                foreach (var teamPlayer in newGame.BlueTeam.TeamPlayers.ToList())
                {
                    var userName = teamPlayer.Player!.UserName;
                    var email = teamPlayer.Player!.Email;
                    var userTeam = newGame.BlueTeam.Name;
                    var opponentTeam = newGame.RedTeam.Name;
                    await _emailSender.SendGameScheduledEmail(email!, userName!, game.Name!, userTeam!, opponentTeam!, game.Date ?? DateTime.MaxValue);
                }

                foreach (var teamPlayer in newGame.RedTeam.TeamPlayers.ToList())
                {
                    var userName = teamPlayer.Player!.UserName;
                    var email = teamPlayer.Player!.Email;
                    var userTeam = newGame.RedTeam.Name;
                    var opponentTeam = newGame.BlueTeam.Name;
                    await _emailSender.SendGameScheduledEmail(email!, userName!, game.Name!, userTeam!, opponentTeam!, game.Date ?? DateTime.MaxValue);
                }


                return CreatedAtAction(nameof(PostGame), new { id = newGame.Id });
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
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest("Something went wrong!");
            }
        }
    }
}
