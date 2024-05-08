using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using Microsoft.AspNetCore.Authorization;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            var result = await _teamService.GetAllTeams();
            return Ok(result);
        }

        [HttpGet("team/{username}")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeamIparticipate(string username)
        {
            var result = await _teamService.GetTeamIparticipate(username);
            return Ok(result);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto?>> GetTeam(int id)
        {
            var result = await _teamService.GetTeam(id);
            return result;
        }

        [HttpGet("Game_History/{id}")]
        public async Task<ActionResult<List<GameDto>>> GameHistory(int id)
        {
            var result = await _teamService.GameHistory(id);
            return result;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, Team team)
        {
            var result = await _teamService.EditTeam(id, team);
            return result;
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TeamDto>> PostTeam(TeamDto team)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _teamService.PostTeam(team, authHeader);
            return result;
        }
        [Authorize]
        [HttpPost("addplayer")]
        public async Task<ActionResult<TeamDto>> AddTeamMember(Dictionary<string, string> dict)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _teamService.AddTeamMember(dict, authHeader);
            return result;
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteTeam(id);
            return result;
        }
    }
}
