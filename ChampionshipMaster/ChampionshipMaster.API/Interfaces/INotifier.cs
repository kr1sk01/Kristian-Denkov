namespace ChampionshipMaster.API.Interfaces
{
    public interface INotifier
    {
        Task SendNotification(string message);
    }
}