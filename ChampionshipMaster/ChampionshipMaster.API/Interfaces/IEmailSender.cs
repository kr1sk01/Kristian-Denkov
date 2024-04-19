namespace ChampionshipMaster.API.Interfaces
{
    public interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string userEmail, string userName, string confirmationLink);
    }
}
