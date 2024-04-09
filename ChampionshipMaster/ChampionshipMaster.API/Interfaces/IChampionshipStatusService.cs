namespace ChampionshipMaster.API.Interfaces
{
    public interface IChampionshipStatusService
    {
        Task<List<ChampionshipStatus>> GetAllChampionshipStatuses();
        Task<ActionResult<ChampionshipStatus?>> GetChampionshipStatus(int id);
        Task<ActionResult<ChampionshipStatus>> PostChampionshipStatus(ChampionshipStatus game);
        Task<bool> ChampionshipStatusNameExists(string? name);
        Task<IActionResult> EditChampionshipStatus(int id, ChampionshipStatus game);
        Task<bool> ChampionshipStatusExists(int id);
        Task<IActionResult> DeleteChampionshipStatus(int id);
    }
}
