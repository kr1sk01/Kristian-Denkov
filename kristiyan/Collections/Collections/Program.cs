namespace MyProject;
class Program
{

    static void Main(string[] args)
    {
        List<Product> products = new List<Product>();

        products.Add(new Product("apple", 1.99, 5));
        products.Add(new Product("pineapple", 2.99, 5));
        products.Add(new Product("cherry",3.99, 2));
        products.Add(new Product("tomato", 4.99, 1));

        foreach(Product p in products)
        {
            p.print();
        }
    }
    static void PrintList(List<string> list) 
    {
        foreach (string s in list)
        {
            Console.WriteLine(s);
        }
    }
}

class Product 
{
    public Product(string name, double price, int quantity) 
    {
        this.name = name;
        this.price =  price;
        this.quantity = quantity;
    }
    private string name;
    private double price;
    private int quantity;

    public void print() 
    {
        Console.WriteLine("Item: {0}, price: {1}, quantity: {2}", this.name, this.price, this.quantity);
    }


}