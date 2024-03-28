using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Championship.API.Models;
using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipStatusController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/ChampionshipStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChampionshipStatus>>> Get()
        {
            return await _context.ChampionshipStatuses.ToListAsync();
        }

        // GET: api/ChampionshipStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChampionshipStatus>> Get(string id)
        {
            var championshipStatus = await _context.ChampionshipStatuses.FirstOrDefaultAsync(cs => cs.Id == id);
            if (championshipStatus == null)
            {
                return NotFound();
            }
            return championshipStatus;
        }

        // POST: api/ChampionshipStatus
        [HttpPost]
        public async Task<ActionResult<ChampionshipStatus>> Post(ChampionshipStatus championshipStatus)
        {
            await _context.ChampionshipStatuses.AddAsync(championshipStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { 
                id = championshipStatus.Id },
                championshipStatus);
        }

        // PUT: api/ChampionshipStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, ChampionshipStatus championshipStatus)
        {
            var existingChampionshipStatus = await _context.ChampionshipStatuses.FirstOrDefaultAsync(cs => cs.Id == id);
            if (existingChampionshipStatus == null)
            {
                return NotFound();
            }
            
            _context.Entry(championshipStatus).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ChampionshipStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {

            var championshipStatus = await _context.ChampionshipStatuses.FirstOrDefaultAsync(cs => cs.Id == id);

            if (championshipStatus == null)
            {
                return NotFound();
            }

            var championship = await _context.Championships.Where(x => x.ChampionshipStatusId == id).ToListAsync();
            
            foreach (var item in championship)
            {
                item.ChampionshipStatusId = null;

                _context.Entry(item).State = EntityState.Modified;
            }
            
            _context.ChampionshipStatuses.Remove(championshipStatus);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
