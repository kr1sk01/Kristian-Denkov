namespace ChampionshipMaster.API.Services.Interfaces
{
    public interface IChampionshipService
    {
        Task<List<ChampionshipDto>> GetAllChampionships();
        Task<List<ChampionshipDto>> GetChampionshipsDetails();
        Task<Championship?> GetChampionship(int id);
        Task PostChampionship(Championship championship);
        Task<bool> ChampionshipNameExists(string name);
    }
}
