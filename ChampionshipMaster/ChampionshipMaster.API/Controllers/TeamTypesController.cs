using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamTypesController : ControllerBase
    {
        private readonly ITeamTypesService _teamTypesService;

        public TeamTypesController(ITeamTypesService teamTypesService)
        {
            _teamTypesService = teamTypesService;
        }

        // GET: api/TeamTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamType>>> GetTeamTypes()
        {
            var result = await _teamTypesService.GetAllTeamTypes();
            return Ok(result);
        }

        // GET: api/TeamTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamType?>> GetTeamType(int id)
        {
            var result = await _teamTypesService.GetTeamType(id);
            return result;
        }

        // PUT: api/TeamTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamType(int id, TeamType teamType)
        {
            var result = await _teamTypesService.EditTeamType(id, teamType);
            return result;
        }

        // POST: api/TeamTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamType>> PostTeamType(TeamType teamType)
        {
            var result = await _teamTypesService.PostTeamType(teamType);
            return result;
        }

        // DELETE: api/TeamTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamType(int id)
        {
            var result = await _teamTypesService.DeleteTeamType(id);
            return result;
        }
    }
}
