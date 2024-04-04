namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipTeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipTeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChampionshipTeams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChampionshipTeams>>> GetChampionshipTeams()
        {
            return await _context.ChampionshipTeams.ToListAsync();
        }

        // GET: api/ChampionshipTeams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChampionshipTeams>> GetChampionshipTeams(int id)
        {
            var championshipTeams = await _context.ChampionshipTeams.FindAsync(id);

            if (championshipTeams == null)
            {
                return NotFound();
            }

            return championshipTeams;
        }

        // PUT: api/ChampionshipTeams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChampionshipTeams(int id, ChampionshipTeams championshipTeams)
        {
            if (id != championshipTeams.Id)
            {
                return BadRequest();
            }

            _context.Entry(championshipTeams).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChampionshipTeamsExists(id))
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

        // POST: api/ChampionshipTeams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChampionshipTeams>> PostChampionshipTeams(ChampionshipTeams championshipTeams)
        {
            _context.ChampionshipTeams.Add(championshipTeams);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChampionshipTeamsExists(championshipTeams.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetChampionshipTeams", new { id = championshipTeams.Id }, championshipTeams);
        }

        // DELETE: api/ChampionshipTeams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChampionshipTeams(int id)
        {
            var championshipTeams = await _context.ChampionshipTeams.FindAsync(id);
            if (championshipTeams == null)
            {
                return NotFound();
            }

            _context.ChampionshipTeams.Remove(championshipTeams);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChampionshipTeamsExists(int id)
        {
            return _context.ChampionshipTeams.Any(e => e.Id == id);
        }
    }
}
