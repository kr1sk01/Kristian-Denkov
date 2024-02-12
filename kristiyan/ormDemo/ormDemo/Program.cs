using ormDemo.Models;

namespace ormDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Getting DB Context to be able to manipulate the DB
            var db = new ApplicationDbContext();
            //Creating the DB if it doesn't currently exist
            db.Database.EnsureCreated();
            //Adding some data to the DB
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
            //Saving all the changes we've done from tracker to the actual DB 
            db.SaveChanges();
        }
    }
}
