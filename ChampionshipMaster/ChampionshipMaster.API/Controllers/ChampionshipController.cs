using ChampionshipMaster.API.Services.Interfaces;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipController : ControllerBase
{
    private readonly IChampionshipService _championshipService;
    private readonly ApplicationDbContext _context;
    public ChampionshipController(IChampionshipService championshipService, ApplicationDbContext context)
    {
        _championshipService = championshipService;
        _context = context;
    }

    // GET: api/Championship
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Championship>>> GetChampionships()
    {
        var result = await _championshipService.GetAllChampionships();
        return Ok(result);
    }

    [HttpGet("details")]
    public async Task<ActionResult<IEnumerable<ChampionshipDto>>> GetChampionshipsDetails()
    {
        var result = await _championshipService.GetChampionshipsDetails();
        return Ok(result);
    }

    // GET: api/Championship/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipDto>> GetChampionship(int id)
    {
        var championship = await _championshipService.GetChampionship(id);

        if (championship == null)
        {
            return NotFound();
        }

        var dto = championship.Adapt<ChampionshipDto>();
        return Ok(dto);
    }

    // POST: api/Championship
    [HttpPost]
    public async Task<ActionResult<Championship>> PostChampionship(Championship championship)
    {
        if (await _championshipService.ChampionshipNameExists(championship.Name))
        {
            return BadRequest("There is already a championship with that name");
        }

        await _championshipService.PostChampionship(championship);

        return CreatedAtAction(nameof(GetChampionship), new
        {
            id = championship.Id
        },
            championship);
    }

    // PUT: api/Championship/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutChampionship(int id, Championship championship)
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

    // DELETE: api/Championship/5
    [HttpDelete("{id}")]
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

    private async Task<bool> ChampionshipExists(int id)
    {
        return await _context.Championships.AnyAsync(e => e.Id == id);
    }
}
