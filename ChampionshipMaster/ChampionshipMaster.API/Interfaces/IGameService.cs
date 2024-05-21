﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ChampionshipMaster.API.Interfaces
{
    public interface IGameService
    {
        Task<List<GameDto>> GetAllGames();
        Task<ActionResult<GameDto?>> GetGame(int id);
        Task<ActionResult<Game>> PostGame(GameDto game, StringValues authHeader);
        Task<IActionResult> EditGame(int id, Game game);
        Task<bool> GameExists(int id);
        Task<IActionResult> DeleteGame(int id);
        Task<ActionResult> ChangeGameName(string gameId, string newName, StringValues authHeader);
    }
}
