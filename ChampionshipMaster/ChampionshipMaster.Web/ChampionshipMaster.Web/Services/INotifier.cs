namespace ChampionshipMaster.Web.Services
{
    public interface INotifier
    {
        void SendErrorNotification(string message, int duration = 3);
        void SendWarningNotification(string message, int duration = 3);
        void SendInformationalNotification(string message, int duration = 3);
        void SendSuccessNotification(string message, int duration = 3);
    }
}
