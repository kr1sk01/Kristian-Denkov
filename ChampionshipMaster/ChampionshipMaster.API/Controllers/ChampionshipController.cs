using ChampionshipMaster.API.Services.Interfaces;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly IChampionshipService _championshipService;

    public ChampionshipController(ApplicationDbContext context, IChampionshipService championshipService)
    {
        _context = context;
        _championshipService = championshipService;
    }

    // GET: api/Championship
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Championship>>> GetChampionshipes()
    {
        var result = await _championshipService.GetAllChampionships();
        return Ok(result);
    }
    [HttpGet("details")]
    public async Task<ActionResult<IEnumerable<ChampionshipDto>>> GetChampionshipesDetails()
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
        return Ok(dtos);
    }

    // GET: api/Championship/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipDto>> GetChampionship(int id)
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

        var dto = championshipClass.Adapt<ChampionshipDto>();
        return Ok(dto);
    }

    // POST: api/Championship
    [HttpPost]
    public async Task<ActionResult<Championship>> PostChampionship(Championship championshipClass)
    {
        _context.Championships.Add(championshipClass);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChampionship), new
        {
            id = championshipClass.Id
        },
            championshipClass);
    }

    // PUT: api/Championship/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChampionship(int id, Championship championshipClass)
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

    // DELETE: api/Championship/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChampionship(int id)
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

    private async Task<bool> ChampionshipExists(int id)
    {
        return await _context.Championships.AnyAsync(e => e.Id == id);
    }
}
