using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;

namespace Championship.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GameStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameStatus>>> GetGameStatuses()
        {
            return await _context.GameStatuses.ToListAsync();
        }

        // GET: api/GameStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameStatus>> GetGameStatus(string id)
        {
            var gameStatus = await _context.GameStatuses.FindAsync(id);

            if (gameStatus == null)
            {
                return NotFound();
            }

            return gameStatus;
        }

        // PUT: api/GameStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameStatus(string id, GameStatus gameStatus)
        {
            if (id != gameStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameStatusExists(id))
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

        // POST: api/GameStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameStatus>> PostGameStatus(GameStatus gameStatus)
        {
            _context.GameStatuses.Add(gameStatus);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GameStatusExists(gameStatus.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGameStatus", new { id = gameStatus.Id }, gameStatus);
        }

        // DELETE: api/GameStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameStatus(string id)
        {
            var gameStatus = await _context.GameStatuses.FindAsync(id);
            if (gameStatus == null)
            {
                return NotFound();
            }

            _context.GameStatuses.Remove(gameStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameStatusExists(string id)
        {
            return _context.GameStatuses.Any(e => e.Id == id);
        }
    }
}
