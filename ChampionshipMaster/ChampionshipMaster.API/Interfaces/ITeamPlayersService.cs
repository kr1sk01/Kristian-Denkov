namespace ChampionshipMaster.API.Interfaces
{
    public interface ITeamPlayersService
    {
        Task<List<TeamPlayersDto>> GetAllTeamPlayers();
        Task<ActionResult<TeamPlayersDto?>> GetTeamPlayers(int id);
        Task<ActionResult<TeamPlayers>> PostTeamPlayers(TeamPlayers teamPlayers);
        Task<IActionResult> EditTeamPlayers(int id, TeamPlayers teamPlayers);
        Task<bool> TeamPlayersExists(int id);
        Task<IActionResult> DeleteTeamPlayers(int id);
    }
}
