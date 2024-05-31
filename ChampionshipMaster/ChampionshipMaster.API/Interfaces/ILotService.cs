namespace ChampionshipMaster.API.Interfaces
{
    public interface ILotService
    {
        int GetPowerOfTwo(int n);
        int GetRound(int teamsCount, int gameIndex);
        bool IsPowerOfTwo(int n);
        int LargestPowerOfTwoLessThan(int n);
        int Rounds(int teamsCount);
        void ShuffleTeams(List<Team> teams);
    }
}
