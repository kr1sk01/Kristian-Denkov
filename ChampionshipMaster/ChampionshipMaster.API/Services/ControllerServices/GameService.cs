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

        public async Task<ActionResult> PostGame(GameDto game, StringValues authHeader)
        {
            try
            {
                var tokenString = authHeader.ToString().Split(' ')[1];
                var token = new JwtSecurityToken(tokenString);

                var userId = token.Claims.First(x => x.Type == "nameid").Value;
                //var userEmail = token.Claims.First(x => x.Type == "email").Value;
                //var userName = token.Claims.First(x => x.Type == "unique_name").Value;

                Game newGame = new Game()
                {
                    Name = game.Name,
                    GameType = await _context.GameTypes.FirstAsync(x => x.Name == game.GameTypeName),
                    BlueTeam = await _context.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).FirstAsync(x => x.Name == game.BlueTeamName),
                    RedTeam = await _context.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).FirstAsync(x => x.Name == game.RedTeamName),
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
                    var userTeam = newGame.BlueTeam.Name;
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
