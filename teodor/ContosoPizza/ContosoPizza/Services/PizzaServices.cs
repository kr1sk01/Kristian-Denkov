using ContosoPizza.Models;

namespace ContosoPizza.Services
{
    public static class PizzaServices
    {
        static List<Pizza> Pizzas { get; }
        static int nextId = 6;
        static PizzaServices()
        {
            Pizzas = new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
                new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true },
                new Pizza { Id = 3, Name = "Pepperoni Lover's", IsGlutenFree = false },
                new Pizza { Id = 4, Name = "Margarita", IsGlutenFree = true },
                new Pizza { Id = 5, Name = "BBQ Chicken", IsGlutenFree = false }
            };
        }
        public static List<Pizza> GetAll() => Pizzas;
        public static Pizza? Get(int id) => Pizzas.FirstOrDefault(x => x.Id == id);
        public static void Add(Pizza pizza)
        {
            pizza.Id = nextId++;
            Pizzas.Add(pizza);
        }
        public static void Delete(int id)
        {
            var pizza = Get(id);
            if (pizza != null) { return; }
            Pizzas.Remove(pizza);
        }
        public static void Update(Pizza pizza)
        {
            var index = Pizzas.FindIndex(x => x.Id == pizza.Id);
            if (index == -1) { return; }
            Pizzas[index] = pizza;
        }
    }
}
