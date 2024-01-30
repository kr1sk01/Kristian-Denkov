namespace Collections_Training_Shopping_LIst
{
    internal class Program
    {
        class ShoppingList
        {
            public string item;
            public double price;
            public int quantity;
        }
        static void Main(string[] args)
        {
            List<ShoppingList> shoppingList= new List<ShoppingList>(10);
            shoppingList.Add(new ShoppingList { item = "Apples", price = 0.95, quantity = 6 });
            shoppingList.Add(new ShoppingList { item = "Milk", price = 2.25, quantity = 1 });
            shoppingList.Add(new ShoppingList { item = "Sugar", price = 1.75, quantity = 2 });
            shoppingList.Add(new ShoppingList { item = "Bread", price = 3.25, quantity = 1 });
            shoppingList.Add(new ShoppingList { item = "Butter", price = 4.95, quantity = 1 });
            shoppingList.Add(new ShoppingList { item = "Cookies", price = 0.5, quantity = 4 });
            shoppingList.Add(new ShoppingList { item = "Oranges", price = 0.65, quantity = 5 });
            shoppingList.Add(new ShoppingList { item = "Chicken", price = 8.95, quantity = 1 });

            PrintShoppingList(shoppingList);
        }

        static void PrintShoppingList(List<ShoppingList> shoppingList)
        {
            foreach (ShoppingList s in shoppingList)
            {
                Console.WriteLine("Item: {0} | Price: ${1} | Quantity: {2}", s.item, s.price.ToString("F2"), s.quantity);
            }
        }
    }
}
