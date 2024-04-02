using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Mapster;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _context.Teams
                .Include(x => x.TeamType)
                .ToListAsync();

            var dto = teams.Adapt<List<TeamDto>>();
            return Ok(dto);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(string id)
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

        [HttpGet("{id}/Game_History")]
        public async Task<ActionResult<TeamDto>> GameHistory(string id)
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

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(string id, Team team)
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
                if (!TeamExists(id))
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

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            _context.Teams.Add(team);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamExists(team.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            var championshipTeamsToDelete = await _context.ChampionshipTeams.Where(c => c.TeamId == id).ToListAsync();

            _context.ChampionshipTeams.RemoveRange(championshipTeamsToDelete);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamExists(string id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
