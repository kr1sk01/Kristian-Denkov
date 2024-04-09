

using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChampionshipTypesController : Controller
{
    private readonly IChampionshipTypeService _championshipTypeService;

    public ChampionshipTypesController(IChampionshipTypeService championshipTypeService)
    {
        _championshipTypeService = championshipTypeService;
    }

    // GET: ChampionshipTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChampionshipType>>> Get()
    {
        var result = await _championshipTypeService.GetAllChampionshipTypes();
        return Ok(result);
    }

    // GET: ChampionshipTypes/Details/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ChampionshipType?>> Get(int id)
    {
        var result = await _championshipTypeService.GetChampionshipType(id);
        return result;
    }

    // POST: ChampionshipTypes
    [HttpPost]
    public async Task<ActionResult<ChampionshipType>> Post(ChampionshipType championshipType)
    {
        var result = await _championshipTypeService.PostChampionshipType(championshipType);
        return result;
    }

    // PUT: ChampionshipTypes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ChampionshipType championshipType)
    {
        var result = await _championshipTypeService.EditChampionshipType(id, championshipType);
        return result;
    }

    // DELETE: api/ChampionshipTypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _championshipTypeService.DeleteChampionshipType(id);
        return result;
    }
}
