using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipTeamsController : ControllerBase
    {
        private readonly IChampionshipTeamsService _championshipTeamsService;

        public ChampionshipTeamsController(IChampionshipTeamsService championshipTeamsService)
        {
            _championshipTeamsService = championshipTeamsService;
        }

        // GET: api/ChampionshipTeams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChampionshipTeamsDto>>> GetChampionshipTeams()
        {
            var result = await _championshipTeamsService.GetAllChampionshipTeams();
            return Ok(result);
        }

        // GET: api/ChampionshipTeams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChampionshipTeamsDto?>> GetChampionshipTeams(int id)
        {
            var result = await _championshipTeamsService.GetChampionshipTeams(id);
            return result;
        }

        // PUT: api/ChampionshipTeams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChampionshipTeams(int id, ChampionshipTeams championshipTeams)
        {
            var result = await _championshipTeamsService.EditChampionshipTeams(id, championshipTeams);
            return result;
        }

        // POST: api/ChampionshipTeams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChampionshipTeams>> PostChampionshipTeams(ChampionshipTeams championshipTeams)
        {
            var result = await _championshipTeamsService.PostChampionshipTeams(championshipTeams);
            return result;
        }

        // DELETE: api/ChampionshipTeams/delete/1/5
        [HttpDelete("delete/{championshipId}/{teamId}")]
        public async Task<IActionResult> DeleteChampionshipTeams(string championshipId, string teamId)
        {
            int championshipId_int = int.Parse(championshipId);
            int teamId_int = int.Parse(teamId);
            var result = await _championshipTeamsService.DeleteChampionshipTeams(championshipId_int, teamId_int);
            return result;
        }
    }
}
