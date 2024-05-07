using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;
using Microsoft.AspNetCore.Authorization;

namespace ChampionshipMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamPlayersController : ControllerBase
    {
        private readonly ITeamPlayersService _teamPLayersService;

        public TeamPlayersController(ITeamPlayersService teamPlayersService)
        {
            _teamPLayersService = teamPlayersService;
        }

        // GET: api/TeamPlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamPlayersDto>>> GetTeamPlayers()
        {
            var result = await _teamPLayersService.GetAllTeamPlayers();
            return Ok(result);
        }

        // GET: api/TeamPlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamPlayersDto?>> GetTeamPlayers(int id)
        {
            var result = await _teamPLayersService.GetTeamPlayers(id);
            return result;
        }

        // PUT: api/TeamPlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamPlayers(int id, TeamPlayers teamPlayers)
        {
            var result = await _teamPLayersService.EditTeamPlayers(id, teamPlayers);
            return result;
        }

        // POST: api/TeamPlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult<TeamPlayers>> PostTeamPlayers(TeamDto team)
        //{
        //    var result = await _teamPLayersService.PostTeamPlayers(team);
        //    return result;
        //}

        // DELETE: api/TeamPlayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamPlayers(int id)
        {
            var result = await _teamPLayersService.DeleteTeamPlayers(id);
            return result;
        }
    }
}
