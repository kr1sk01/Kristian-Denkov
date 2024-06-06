namespace ChampionshipMaster.Web.Services
{
    public class ClientLotService : IClientLotService
    {
        public int GetPowerOfTwo(int n)
        {
            int power = 0;
            while (n > 1)
            {
                n >>= 1; // Right shift by 1 (equivalent to n = n / 2)
                power++;
            }

            return power;
        }

        public int GetRound(int teamsCount, int gameIndex)
        {
            if (IsPowerOfTwo(teamsCount))
            {
                int round = 1;
                for (int i = teamsCount / 2; i < teamsCount; i += (teamsCount - i) / 2)
                {
                    if (gameIndex <= i)
                    {
                        return round;
                    }
                    round++;
                }

                return 0;
            }
            else
            {
                int p = LargestPowerOfTwoLessThan(teamsCount);

                if (gameIndex <= teamsCount - p) { return 1; }

                int round = 2;
                for (int i = p / 2; i < p; i += (p - i) / 2)
                {
                    if (gameIndex <= (teamsCount - p) + i)
                    {
                        return round;
                    }
                    round++;
                }

                return 0;
            }
        }

        public bool IsPowerOfTwo(int n)
        {
            // A number is a power of two if it's greater than 0 and only has one bit set to 1.
            // (n & (n - 1)) will be 0 if n is a power of two.
            return (n > 0) && ((n & (n - 1)) == 0);
        }

        public int LargestPowerOfTwoLessThan(int n)
        {
            // Find the largest power of 2 less than n
            int power = 1;
            while (power < n)
            {
                power <<= 1; // Multiply power by 2
            }

            return power >> 1; // Divide power by 2 to get the largest power of 2 less than n
        }

        public int Rounds(int teamsCount)
        {
            if (IsPowerOfTwo(teamsCount))
            {
                return GetPowerOfTwo(teamsCount);
            }
            else
            {
                int p = LargestPowerOfTwoLessThan(teamsCount);
                return GetPowerOfTwo(p) + 1;
            }
        }

        public int GamesInRound(int teamsCount, int round)
        {
            if (IsPowerOfTwo(teamsCount))
            {
                int currentRound = 1;
                for (int i = teamsCount / 2; i < teamsCount; i += (teamsCount - i) / 2)
                {
                    if (currentRound == round) { return teamsCount - i; }
                    currentRound++;
                }

                return 0;
            }
            else
            {
                int p = LargestPowerOfTwoLessThan(teamsCount);

                if (round == 1) { return teamsCount - p; }

                int currentRound = 2;
                for (int i = p / 2; i < p; i += (p - i) / 2)
                {
                    if (currentRound == round) { return p - i; }
                    currentRound++;
                }

                return 0;
            }
        }
    }
}
