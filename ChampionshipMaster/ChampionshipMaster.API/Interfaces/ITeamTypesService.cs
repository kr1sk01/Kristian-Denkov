namespace ChampionshipMaster.API.Interfaces
{
    public interface ITeamTypesService
    {
        Task<List<TeamType>> GetAllTeamTypes();
        Task<ActionResult<TeamType?>> GetTeamType(int id);
        Task<ActionResult<TeamType>> PostTeamType(TeamType teamType);
        Task<bool> TeamTypeNameExists(string? name);
        Task<IActionResult> EditTeamType(int id, TeamType teamType);
        Task<bool> TeamTypeExists(int id);
        Task<IActionResult> DeleteTeamType(int id);
    }
}
