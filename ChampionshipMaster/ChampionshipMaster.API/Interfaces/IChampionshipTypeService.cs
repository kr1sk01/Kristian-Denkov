namespace ChampionshipMaster.API.Interfaces
{
    public interface IChampionshipTypeService
    {
        Task<List<ChampionshipTypeDto>> GetAllChampionshipTypes();
        Task<ActionResult<ChampionshipType?>> GetChampionshipType(int id);
        Task<ActionResult<ChampionshipType>> PostChampionshipType(ChampionshipType championshipType);
        Task<bool> ChampionshipTypeNameExists(string? name);
        Task<IActionResult> EditChampionshipType(int id, ChampionshipType championshipType);
        Task<bool> ChampionshipTypeExists(int id);
        Task<IActionResult> DeleteChampionshipType(int id);
    }
}
