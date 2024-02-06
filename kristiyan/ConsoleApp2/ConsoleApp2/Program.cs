namespace ConsoleApp2
{
    using System.Security.Principal;
    using System.Threading;

    namespace Thread_Synchronization_Practice
    {
        class Account
        {
            static Semaphore _semaphore = new Semaphore(1, 1);
            private int balance;

            public Account(int initialBalance)
            {
                this.balance = initialBalance;
            }

            public void Deposit(int amount)
            {
                // Implement synchronization here to protect balance during deposit
                // Update balance and print the new balance
                Console.WriteLine("Deposit[{0}] has been initialized.", Thread.CurrentThread.ManagedThreadId);
                _semaphore.WaitOne();
                this.balance += amount;
                Thread.Sleep(2000);
                Console.WriteLine("Deposit[{0}] successful. Balance updated. New balance is: {1}", Thread.CurrentThread.ManagedThreadId, this.balance);
                _semaphore.Release();
            }
            public void Withdraw(int amount)
            {
                // Implement synchronization here to protect balance during withdrawal
                // Update balance and print the new balance
                Console.WriteLine("Withdraw[{0}] has been initialized.", Thread.CurrentThread.ManagedThreadId);
                _semaphore.WaitOne();
                this.balance -= amount;
                Thread.Sleep(2000);
                Console.WriteLine("Withdraw[{0}] successful. Balance updated. New balance is: {1}", Thread.CurrentThread.ManagedThreadId, this.balance);
                _semaphore.Release();
            }

            public int GetBalance()
            {
                return balance;
            }
        }
        internal class Program
        {
            static void Main(string[] args)
            {
                // Create an account with an initial balance
                Account account = new Account(1000);

                // Create multiple threads representing concurrent transactions
                Thread depositThread = new Thread(() => DepositTransactions(account));
                Thread withdrawThread = new Thread(() => WithdrawTransactions(account));

                depositThread.Start();
                withdrawThread.Start();

                depositThread.Join();
                withdrawThread.Join();
                // Display the final account balance
                Console.WriteLine("Final Account Balance: " + account.GetBalance());
                
            }
            static void DepositTransactions(Account account)
            {
                // Implement multiple deposit transactions
                for (int i = 0; i < 5; i++)
                {
                    Thread t = new Thread(() => account.Deposit(100));
                    t.Start();
                    t.Join();
                }
            }

            static void WithdrawTransactions(Account account)
            {
                // Implement multiple withdrawal transactions
                for (int i = 0; i < 5; i++)
                {
                    Thread t = new Thread(() => account.Withdraw(50));
                    t.Start();
                    t.Join();
                }
            }
        }
    }
}
