namespace ChampionshipMaster.Web.Services
{
    public interface INotifier
    {
        void SendErrorMessage(string message);
        void SendWarningMessage(string message);
        void SendInformationalMessage(string message);
        void SendSuccessMessage(string message);
    }
}
