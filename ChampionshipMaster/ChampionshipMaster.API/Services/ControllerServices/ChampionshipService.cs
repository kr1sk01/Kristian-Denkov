﻿using ChampionshipMaster.API.Interfaces;
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
                    ChampionshipStatus = await _context.ChampionshipStatuses.FirstAsync(x => x.Name == championship.ChampionshipStatusName),
                    GameType = await _context.GameTypes.FirstAsync(x => x.Name == championship.GameTypeName),
                    LotDate = championship.LotDate,
                    Date = championship.Date,
                    ModifiedBy = userId,
                    ModifiedOn = DateTime.UtcNow
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
