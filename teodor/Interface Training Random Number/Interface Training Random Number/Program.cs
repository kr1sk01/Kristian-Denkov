namespace Interface_Training_Random_Number
{
    internal class Program
    {
        interface IRandomizable
        {
            double GetRandomNum(int upperBound);
        }

        class myClass : IRandomizable
        {
            public myClass()
            {
            }

            public double GetRandomNum(int upperBound)
            {
                Random random = new Random();
                return random.NextDouble()*upperBound;
            }
        }
        static void Main(string[] args)
        {
            myClass myClass = new myClass();

            while (true)
            {
                Console.Write("Enter the upper bound: ");
                string? input = Console.ReadLine();

                if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                else if(int.TryParse(input, out int upperBound))
                {
                    Console.WriteLine("Your random number is: {0}", myClass.GetRandomNum(upperBound));
                }
                else
                {
                    Console.WriteLine("Your input is invalid.");
                }
            }
        }
    }
}
