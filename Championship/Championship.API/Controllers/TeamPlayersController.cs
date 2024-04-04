using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamPlayersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamPlayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamPlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamPlayers>>> GetTeamPlayers()
        {
            return await _context.TeamPlayers.ToListAsync();
        }

        // GET: api/TeamPlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamPlayers>> GetTeamPlayers(int id)
        {
            var teamPlayers = await _context.TeamPlayers.FindAsync(id);

            if (teamPlayers == null)
            {
                return NotFound();
            }

            return teamPlayers;
        }

        // PUT: api/TeamPlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamPlayers(int id, TeamPlayers teamPlayers)
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
                if (!TeamPlayersExists(id))
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

        // POST: api/TeamPlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamPlayers>> PostTeamPlayers(TeamPlayers teamPlayers)
        {
            _context.TeamPlayers.Add(teamPlayers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamPlayersExists(teamPlayers.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeamPlayers", new { id = teamPlayers.Id }, teamPlayers);
        }

        // DELETE: api/TeamPlayers/5
        [HttpDelete("{id}")]
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

        private bool TeamPlayersExists(int id)
        {
            return _context.TeamPlayers.Any(e => e.Id == id);
        }
    }
}
