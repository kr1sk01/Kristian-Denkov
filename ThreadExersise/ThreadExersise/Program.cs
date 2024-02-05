
internal class Program
{
    public static Semaphore s = new Semaphore(1,1);
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        //Exersise 1
        new Thread(Hello1).Start();
        new Thread(Hello2).Start();
        
        //Exersise 2
        for (int i = 0; i < 5; i++)
        {
            new Thread(Write).Start();
        }
    }

    static void Hello1()
    {
        Console.WriteLine($"Hello {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(2000);
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished");

    }
    static void Hello2()
    {
        Console.WriteLine($"Hello {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1000);
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished");
    }
    static void Write() 
    {
        Console.WriteLine($"Chakam da si pisha batko s (xD) {Thread.CurrentThread.ManagedThreadId}");
        s.WaitOne();
        Console.WriteLine($"Epa pushem si s (xD) {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1000);
        Console.WriteLine($"Epa spreh da pisha s (xD) {Thread.CurrentThread.ManagedThreadId}");
        s.Release();
    }
}