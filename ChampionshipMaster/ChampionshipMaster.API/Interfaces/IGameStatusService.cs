namespace ChampionshipMaster.API.Interfaces
{
    public interface IGameStatusService
    {
        Task<List<GameStatus>> GetAllGameStatuses();
        Task<ActionResult<GameStatus?>> GetGameStatus(int id);
        Task<ActionResult<GameStatus>> PostGameStatus(GameStatus game);
        Task<bool> GameStatusNameExists(string? name);
        Task<IActionResult> EditGameStatus(int id, GameStatus game);
        Task<bool> GameStatusExists(int id);
        Task<IActionResult> DeleteGameStatus(int id);
    }
}
