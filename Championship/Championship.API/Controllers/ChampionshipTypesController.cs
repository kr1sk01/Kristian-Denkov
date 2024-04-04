using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChampionshipTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChampionshipType>>> Get()
        {
            return await _context.ChampionshipTypes.ToListAsync();
        }

        // GET: ChampionshipTypes/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChampionshipType>> Get(int id)
        {
            var championshipType = await _context.ChampionshipTypes.FirstOrDefaultAsync(cs => cs.Id == id);
            if (championshipType == null)
            {
                return NotFound();
            }
            return championshipType;
        }

        // POST: ChampionshipTypes
        [HttpPost]
        public async Task<ActionResult<ChampionshipType>> Post(ChampionshipType championshipType)
        {
            await _context.ChampionshipTypes.AddAsync(championshipType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new
            {
                id = championshipType.Id
            },
                championshipType);
        }

        // PUT: ChampionshipTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ChampionshipType championshipType)
        {
            var existingChampionshipType = await _context.ChampionshipTypes.FirstOrDefaultAsync(cs => cs.Id == id);
            if (existingChampionshipType == null)
            {
                return NotFound();
            }

            _context.Entry(championshipType).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ChampionshipTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var championshipType = await _context.ChampionshipTypes.FirstOrDefaultAsync(cs => cs.Id == id);

            if (championshipType == null)
            {
                return NotFound();
            }

            var championship = await _context.Championships.Where(x => x.ChampionshipTypeId == id).ToListAsync();

            foreach (var item in championship)
            {
                item.ChampionshipTypeId = null;

                _context.Entry(item).State = EntityState.Modified;
            }

            _context.ChampionshipTypes.Remove(championshipType);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
