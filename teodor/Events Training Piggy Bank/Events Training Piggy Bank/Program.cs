using System.Runtime.CompilerServices;

namespace Events_Training_Piggy_Bank
{
    delegate void BalanceGoalEventHandler(int i);
    class PiggyBank
    {
        private static int balance = 0;

        public event BalanceGoalEventHandler? reachedBalanceGoal;
        public void MainLoop()
        {
            string? input;
            while (true)
            {
                Console.WriteLine("How much to deposit?");
                input = Console.ReadLine();
                if(input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                else if(int.TryParse(input, out int depositAmount))
                {
                    balance += depositAmount;
                    if (balance >= 500)
                    {
                        this.reachedBalanceGoal(balance);
                    }
                    Console.WriteLine("The balance amount is: {0}", balance);
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            PiggyBank obj = new PiggyBank();
            obj.reachedBalanceGoal += new BalanceGoalEventHandler(obj_reachedBalanceGoal);

            obj.MainLoop();
        }

        static void obj_reachedBalanceGoal(int i)
        {
            Console.WriteLine("You have reached your savings goal! You have {0}", i);
        }
    }
}
