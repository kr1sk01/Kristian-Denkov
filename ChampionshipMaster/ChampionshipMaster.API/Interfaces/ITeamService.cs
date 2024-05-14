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
        Task<ActionResult<TeamDto>> SetTeamMembers(string teamId, List<string> playerIds, StringValues authHeader);
        Task<IActionResult> EditTeam(int id, Team team);
        Task<bool> TeamExists(int id);
        Task<bool> TeamNameExists(string? name);
        Task<IActionResult> DeleteTeam(int id);
        Task<ActionResult> ChangeTeamName(string teamId, string newName, StringValues authHeader);
        Task<ActionResult> ChangeTeamLogo(string teamId, string newLogo, StringValues authHeader);
    }
}
