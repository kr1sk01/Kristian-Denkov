namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipClassController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ChampionshipClassController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ChampionshipClass
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChampionshipClass>>> GetChampionshipClasses([FromQuery] string? championshipStatusName)
    {
        var championships = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .ToListAsync();

        var dto = championships.Adapt<List<ChampionshipClassDto>>();
        if (string.IsNullOrEmpty(championshipStatusName))
            return Ok(dto);

        var filteredDto = dto.Where(x => x.ChampionshipStatusName == championshipStatusName).ToList();
        return Ok(filteredDto);
    }
    [HttpGet("details")]
    public async Task<ActionResult<IEnumerable<ChampionshipClassDto>>> GetChampionshipClassesDetails()
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

        var dtos = championshipClassesWithDetails.Adapt<List<ChampionshipClassDto>>();
        return Ok(dtos);
    }

    // GET: api/ChampionshipClass/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipClassDto>> GetChampionshipClass(int id)
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

        if (championshipClass == null)
        {
            return NotFound();
        }

        var dto = championshipClass.Adapt<ChampionshipClassDto>();
        return Ok(dto);
    }

    // POST: api/ChampionshipClass
    [HttpPost]
    public async Task<ActionResult<ChampionshipClass>> PostChampionshipClass(ChampionshipClass championshipClass)
    {
        _context.Championships.Add(championshipClass);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChampionshipClass), new
        {
            id = championshipClass.Id
        },
            championshipClass);
    }

    // PUT: api/ChampionshipClass/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChampionshipClass(int id, ChampionshipClass championshipClass)
    {
        if (id != championshipClass.Id)
        {
            return BadRequest();
        }

        _context.Entry(championshipClass).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ChampionshipClassExists(id))
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

    // DELETE: api/ChampionshipClass/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChampionshipClass(int id)
    {
        var championshipClass = await _context.Championships.FindAsync(id);
        if (championshipClass == null)
        {
            return NotFound();
        }

        var championshipTeamsToDelete = await _context.ChampionshipTeams.Where(c => c.ChampionshipId == id).ToListAsync();

        _context.ChampionshipTeams.RemoveRange(championshipTeamsToDelete);
        _context.Championships.Remove(championshipClass);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ChampionshipClassExists(int id)
    {
        return await _context.Championships.AnyAsync(e => e.Id == id);
    }
}
