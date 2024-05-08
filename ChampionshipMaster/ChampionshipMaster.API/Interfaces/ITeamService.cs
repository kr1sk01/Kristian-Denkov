using Microsoft.Extensions.Primitives;

namespace ChampionshipMaster.API.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamDto>> GetAllTeams();
        Task<List<TeamDto>> GetTeamIparticipate(string username);
        Task<ActionResult<TeamDto?>> GetTeam(int id);
        Task<ActionResult<List<GameDto>>> GameHistory(int id);
        Task<ActionResult<TeamDto>> PostTeam(TeamDto team, StringValues authHeader);
        Task<ActionResult<TeamDto>> AddTeamMember(TeamDto team, StringValues authHeader);
        Task<IActionResult> EditTeam(int id, Team team);
        Task<bool> TeamExists(int id);
        Task<bool> TeamNameExists(string? name);
        Task<IActionResult> DeleteTeam(int id);
    }
}
