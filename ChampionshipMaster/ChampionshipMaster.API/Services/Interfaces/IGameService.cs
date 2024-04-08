using Microsoft.AspNetCore.Mvc;

namespace GameMaster.API.Services.Interfaces
{
    public interface IGameService
    {
        Task<List<GameDto>> GetAllGames();
        Task<ActionResult<GameDto?>> GetGame(int id);
        Task<ActionResult<Game>> PostGame(Game game);
        Task<IActionResult> EditGame(int id, Game game);
        Task<bool> GameExists(int id);
        Task<IActionResult> DeleteGame(int id);
    }
}
