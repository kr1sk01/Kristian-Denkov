using ormDemo.Models;

namespace ormDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            db.Database.EnsureCreated();
            
            db.Categories.Add(new Category {
                Title = "Sport",
                News = new List<News>
                {
                    new News
                    {
                        Title = "CSKA bie levski",
                        Content = "ЦСКА БИЕ ЛЕВСКИ",
                        Comments = new List<Comment>
                        {
                            new Comment { Author = "Niki", Content = "da"},
                            new Comment { Author="Stoyan", Content="ne"}
                        }
                    }
                }
            }) ;
            db.SaveChanges();
        }
    }
}
