namespace ChampionshipMaster.Web.Services
{
    public interface IClientLotService
    {
        int Rounds(int teamsCount);
        int GetRound(int teamsCount, int gameIndex);
        bool IsPowerOfTwo(int n);
        int GetPowerOfTwo(int n);
        int LargestPowerOfTwoLessThan(int n);
        int GamesInRound(int teamsCount, int round);
    }
}
