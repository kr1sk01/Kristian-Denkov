using EF_Training_Student_Enrollment.Models;
using Microsoft.EntityFrameworkCore;
using EF_Training_Student_Enrollment.Services;

namespace EF_Training_Student_Enrollment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            db.Database.EnsureCreated();
            //PopulateDb(db);
            StudentServices.PrintInfo(db, 10);
            Console.WriteLine("-------");
            CourseServices.PrintInfo(db, 3);
        }

        //Function for generating some starter data
        static void PopulateDb(ApplicationDbContext db)
        {
            var students = new List<Student>
            {
                new Student { Name = "John Doe", DateOfBirth = new DateOnly(2000, 1, 1), Email = "john@example.com" },
                new Student { Name = "Alice Smith", DateOfBirth = new DateOnly(1999, 5, 15), Email = "alice@example.com" },
                new Student { Name = "Bob Johnson", DateOfBirth = new DateOnly(2001, 9, 20), Email = "bob@example.com" },
                new Student { Name = "Emily Brown", DateOfBirth = new DateOnly(1998, 3, 10), Email = "emily@example.com" },
                new Student { Name = "Michael Wilson", DateOfBirth = new DateOnly(2002, 7, 5), Email = "michael@example.com" },
                new Student { Name = "Sophia Lee", DateOfBirth = new DateOnly(2003, 11, 25), Email = "sophia@example.com" },
                new Student { Name = "William Miller", DateOfBirth = new DateOnly(1997, 12, 30), Email = "william@example.com" },
                new Student { Name = "Emma Davis", DateOfBirth = new DateOnly(2004, 4, 18), Email = "emma@example.com" },
                new Student { Name = "James Wilson", DateOfBirth = new DateOnly(1996, 8, 8), Email = "james@example.com" },
                new Student { Name = "Olivia Martinez", DateOfBirth = new DateOnly(2005, 6, 3), Email = "olivia@example.com" }
            };

            //Adding students to database
            db.Students.AddRange(students);
            db.SaveChanges();

            var courses = new List<Course>
            {
                new Course { Title = "Mathematics", Description = "Introduction to mathematics" },
                new Course { Title = "English", Description = "English language and literature" },
                new Course { Title = "Science", Description = "Basic principles of science" },
                new Course { Title = "History", Description = "Overview of world history" },
                new Course { Title = "Programming", Description = "Fundamentals of programming" }
            };

            //Adding courses to database
            db.Courses.AddRange(courses);
            db.SaveChanges();

            //Randomly enrolling students in courses
            Random randomizer = new Random();
            int randInt;
            bool addCourse;

            foreach (var student in students)
            {
                foreach (var course in courses)
                {
                    randInt = randomizer.Next(0, 2);
                    addCourse = Convert.ToBoolean(randInt);

                    if (addCourse)
                    {
                        student.courses.Add(course);
                    }
                }
            }

            db.SaveChanges();
        }
    }
}
