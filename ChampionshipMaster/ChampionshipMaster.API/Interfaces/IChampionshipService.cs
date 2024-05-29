namespace ChampionshipMaster.API.Interfaces
{
    public interface IChampionshipService
    {
        Task<List<ChampionshipDto>> GetAllChampionships();
        Task<List<ChampionshipDto>> GetChampionshipsDetails();
        Task<Championship?> GetChampionship(int id);
        Task<ActionResult> PostChampionship(ChampionshipDto championship, StringValues authHeader);
        Task<bool> ChampionshipNameExists(string name);
        Task<IActionResult> EditChampionship(int id, Championship championship);
        Task<bool> ChampionshipExists(int id);
        Task<IActionResult> DeleteChampionship(int id);
        Task<IActionResult> JoinChampionship(ChampionshipTeamsDto championshipTeam, StringValues authHeader);
    }
}
