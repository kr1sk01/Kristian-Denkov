﻿namespace ChampionshipMaster.API.Services.Interfaces
{
    public interface IChampionshipService
    {
        Task<List<ChampionshipDto>> GetAllChampionships();
        Task<List<ChampionshipDto>> GetChampionshipsDetails();
        Task<Championship?> GetChampionship(int id);
        Task<ActionResult<Championship>> PostChampionship(Championship championship);
        Task<bool> ChampionshipNameExists(string name);
        Task<IActionResult> EditChampionship(int id, Championship championship);
        Task<bool> ChampionshipExists(int id);
        Task<IActionResult> DeleteChampionship(int id);
    }
}
