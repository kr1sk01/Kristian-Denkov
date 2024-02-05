internal class Delegates
{
    public delegate string operati(int a, int b);
    public delegate int deldel(int a, int b);
    private static void Main(string[] args)
    {
        //Delegates test
        operati op = sum;
        Console.WriteLine(op(20,20));
        op = multip;
        Console.WriteLine(op(20,20));
        //Inline delegate
        op = delegate (int a, int b) { return (a / b).ToString(); };
        Console.WriteLine(op(20,20));
        //Composable delegates
        deldel f1, f2, f1f2;
        f1 = sumint;
        Console.WriteLine(f1(10,10));
        f2 = multipint;
        Console.WriteLine(f2(10, 10));
        f1f2 = f1 + f2;
        Console.WriteLine(f1f2(10, 10));
    }
    static int sumint(int a, int b) { return (a + b); }
    static int multipint(int a, int b) { return (a * b); }
    static string sum(int a, int b) { return (a + b).ToString(); }
    static string multip(int a, int b) { return (a * b).ToString(); }
}