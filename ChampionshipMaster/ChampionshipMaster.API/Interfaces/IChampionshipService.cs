using ChampionshipMaster.SHARED.DTO;

namespace ChampionshipMaster.API.Interfaces
{
    public interface IChampionshipService
    {
        Task<List<ChampionshipDto>> GetAllChampionships();
        Task<List<ChampionshipDto>> GetChampionshipsDetails();
        Task<ChampionshipDto?> GetChampionship(int id);
        Task<ActionResult> PostChampionship(ChampionshipDto championship, StringValues authHeader);
        Task<bool> ChampionshipNameExists(string name);
        //Task<IActionResult> EditChampionship(int id, Championship championship);
        Task<IActionResult> EditChampionship(string championshipId, ChampionshipDto championship, StringValues authHeader);
        Task<IActionResult> DrawLot(int championshipId, StringValues authHeader);
        Task<bool> ChampionshipExists(int id);
        Task<IActionResult> DeleteChampionship(int id);
        Task<IActionResult> JoinChampionship(ChampionshipTeamsDto championshipTeam, StringValues authHeader);
        List<Player?> GetChampionshipPlayers(Championship championship);
        Task SendLotNotifications(Championship championship);
    }
}
