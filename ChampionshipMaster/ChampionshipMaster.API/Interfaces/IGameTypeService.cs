namespace ChampionshipMaster.API.Interfaces
{
    public interface IGameTypeService
    {
        Task<List<GameTypeDto>> GetAllGameTypes();
        Task<ActionResult<GameTypeDto?>> GetGameType(int id);
        Task<ActionResult<GameType>> PostGameType(GameType gameType);
        Task<bool> GameTypeNameExists(string? name);
        Task<IActionResult> EditGameType(int id, GameType gameType);
        Task<bool> GameTypeExists(int id);
        Task<IActionResult> DeleteGameType(int id);
    }
}
