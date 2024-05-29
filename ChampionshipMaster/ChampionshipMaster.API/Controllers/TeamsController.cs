using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.API.Services.ControllerServices;
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

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllActiveTeams()
        {
            var result = await _teamService.GetAllActiveTeams();
            return Ok(result);
        }

        [HttpGet("teams/{userId}")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeamsPlayerParticipation(string userId)
        {
            var result = await _teamService.GetAllTeamsPlayerParticipation(userId);
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
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTeam(int id, Team team)
        //{
        //    var result = await _teamService.EditTeam(id, team);
        //    return result;
        //}

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
        //[HttpPost("setPlayers")]
        //public async Task<ActionResult<TeamDto>> SetTeamMembers([FromQuery] string teamId, [FromBody] List<string> playerIds)
        //{
        //    var authHeader = HttpContext.Request.Headers.Authorization;
        //    var result = await _teamService.SetTeamMembers(teamId, playerIds, authHeader);
        //    return result;
        //}

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteTeam(id);
            return result;
        }

        //[Authorize]
        //[HttpPost("changeTeamName")]
        //public async Task<IActionResult> ChangeTeamName([FromQuery] string teamId, [FromBody] Dictionary<string, string> content)
        //{
        //    var authHeader = HttpContext.Request.Headers.Authorization;
        //    string newName = content.FirstOrDefault().Value;
        //    var result = await _teamService.ChangeTeamName(teamId, newName, authHeader);
        //    return result;
        //}

        //[Authorize]
        //[HttpPost("changeTeamLogo")]
        //public async Task<IActionResult> ChangeTeamLogo([FromQuery] string teamId, [FromBody] Dictionary<string, string> content)
        //{
        //    var authHeader = HttpContext.Request.Headers.Authorization;
        //    string newLogo = content.FirstOrDefault().Value;
        //    var result = await _teamService.ChangeTeamLogo(teamId, newLogo, authHeader);
        //    return result;
        //}

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditPlayer([FromQuery] string teamId, [FromBody] TeamDto team)
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            var result = await _teamService.EditTeam(teamId, team, authHeader);
            return result;
        }
    }
}
