namespace ChampionshipMaster.API.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamDto>> GetAllTeams();
        Task<ActionResult<TeamDto?>> GetTeam(int id);
        Task<ActionResult<List<GameDto>>> GameHistory(int id);
        Task<ActionResult<Team>> PostTeam(Team team);
        Task<IActionResult> EditTeam(int id, Team team);
        Task<bool> TeamExists(int id);
        Task<bool> TeamNameExists(string? name);
        Task<IActionResult> DeleteTeam(int id);
    }
}
