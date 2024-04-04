using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;
using Mapster;
using Championship.SHARED.DTO;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GameTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameType>>> GetGameTypes()
        {
            var gameTypes = await _context.GameTypes
                .Include(x => x.TeamType)
                .ToListAsync();

            var dto = gameTypes.Adapt<List<GameTypeDto>>();
            return Ok(dto);
        }

        // GET: api/GameTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameType>> GetGameType(int id)
        {
            var gameType = await _context.GameTypes
                .Include(x => x.TeamType)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (gameType == null)
            {
                return NotFound();
            }

            var dto = gameType.Adapt<GameTypeDto>();
            return Ok(dto);
        }

        // PUT: api/GameTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameType(int id, GameType gameType)
        {
            if (id != gameType.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameTypeExists(id))
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

        // POST: api/GameTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameType>> PostGameType(GameType gameType)
        {
            _context.GameTypes.Add(gameType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GameTypeExists(gameType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGameType", new { id = gameType.Id }, gameType);
        }

        // DELETE: api/GameTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameType(int id)
        {
            var gameType = await _context.GameTypes.FindAsync(id);
            if (gameType == null)
            {
                return NotFound();
            }

            var games = await _context.Games.Where(x => x.GameTypeId == id).ToListAsync();

            foreach (var game in games)
            {
                game.GameTypeId = null;
                _context.Entry(game).State = EntityState.Modified;
            }

            _context.GameTypes.Remove(gameType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameTypeExists(int id)
        {
            return _context.GameTypes.Any(e => e.Id == id);
        }
    }
}
