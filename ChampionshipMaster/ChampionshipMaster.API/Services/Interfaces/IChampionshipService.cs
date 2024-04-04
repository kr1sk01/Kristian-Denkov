namespace ChampionshipMaster.API.Services.Interfaces
{
    public interface IChampionshipService
    {
        Task<List<ChampionshipDto>> GetAllChampionships();
    }
}
