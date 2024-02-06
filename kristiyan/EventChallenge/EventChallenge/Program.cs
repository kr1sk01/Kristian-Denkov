using System;

namespace Program
{
    public delegate void BalanceEventHandler(double theValue);

    class PiggyBank
    {
        private double m_bankBalance;
        public event BalanceEventHandler balanceChanged;

        public double theBalance
        {
            set
            {
                m_bankBalance = value;
                balanceChanged(value);
            }
            get
            {
                return m_bankBalance;
            }
        }
    }

    class BalanceWatcher
    {
        public void balanceWatch(double amount)
        {
            if (amount >= 500)
                Console.WriteLine("You reached your savings goal! You have {0}", amount);
            Console.WriteLine("The balance amount is {0}", amount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PiggyBank pb = new PiggyBank();

            BalanceWatcher bw = new BalanceWatcher();

            pb.balanceChanged += bw.balanceWatch;

            string theStr;
            do
            {
                Console.WriteLine("How much to deposit?");

                theStr = Console.ReadLine();
                if (!theStr.Equals("exit"))
                {
                    double newVal = double.Parse(theStr);
                    pb.theBalance += newVal;
                }
            } while (!theStr.Equals("exit"));
        }
    }
}
