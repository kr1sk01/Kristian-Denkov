namespace ChampionshipMaster.API.Interfaces
{
    public interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string userEmail, string userName, string confirmationLink);
        Task SendGameScheduledEmail(string userEmail, string userName, string gameName, string userTeam, string opponentTeam, DateTime date);
    }
}
