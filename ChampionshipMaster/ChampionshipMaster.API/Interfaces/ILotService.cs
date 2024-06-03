namespace ChampionshipMaster.API.Interfaces
{
    public interface ILotService
    {
        int GamesInRound(int teamsCount, int round);
        int GetNextGameIndex(int teamsCount, int gameIndex, out string side);
        int GetPowerOfTwo(int n);
        int GetRound(int teamsCount, int gameIndex);
        bool IsPowerOfTwo(int n);
        int LargestPowerOfTwoLessThan(int n);
        int Rounds(int teamsCount);
        void ShuffleTeams(List<Team> teams);
    }
}
