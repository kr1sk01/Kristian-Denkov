using DelegatesChallenge;

internal class Program
{
    public delegate double taxDel(double a);
    static void Main(string[] args)
    {
        ShippingFeesDelegate theDel;
        Console.WriteLine("Enter price: ");

        double price;
        price = double.Parse(Console.ReadLine());

        taxDel td = zone1;
        
    }
    static double zone1(double a) 
    {
        return a / 4;
    }
    static double zone2(double a)
    {
        return 25 + ((a * 12) / 100);
    }
    static double zone3(double a)
    {
        return (a * 8)/ 100;
    }
    static double zone4(double a)
    {
        return 25 + ((a * 4) / 100);
    }
}