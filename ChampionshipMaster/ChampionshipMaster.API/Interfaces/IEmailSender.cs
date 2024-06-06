
namespace ChampionshipMaster.API.Interfaces
{
    public interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string userEmail, string userName, string confirmationLink);
        Task SendGameScheduledEmail(string userEmail, string userName, string gameName, string userTeam, string opponentTeam, DateTime date);
        Task SendGameFinishedEmail(string userEmail, string gameName, string blueTeam, int bluePoints, string redTeam, int redPoints);
        Task SendAddedToTeamEmail(string userEmail, string userName, string teamName, string createdBy, string teamType, List<string> players);
        Task SendChampionshipLotEmail(string userEmail, string championshipName, int championshipId, string userName, string userTeam, string opponentTeam, DateTime? date);
    }
}
