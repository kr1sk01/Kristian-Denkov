namespace ChampionshipMaster.API.Interfaces
{
    public interface IChampionshipTeamsService
    {
        Task<List<ChampionshipTeamsDto>> GetAllChampionshipTeams();
        Task<ActionResult<ChampionshipTeamsDto?>> GetChampionshipTeams(int id);
        Task<ActionResult<ChampionshipTeams>> PostChampionshipTeams(ChampionshipTeams championshipTeams);
        Task<IActionResult> EditChampionshipTeams(int id, ChampionshipTeams championshipTeams);
        Task<bool> ChampionshipTeamsExists(int id);
        Task<IActionResult> DeleteChampionshipTeams(int id);
    }
}
