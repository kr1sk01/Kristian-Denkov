using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EF_Training_Student_Enrollment.Models;

namespace EF_Training_Student_Enrollment.Services
{
    public static class CourseServices
    {
        public static Course Get(ApplicationDbContext db, int courseId)
        {
            return db.Courses.FirstOrDefault(c => c.Id == courseId);
        }

        public static List<Course> GetAll(ApplicationDbContext db)
        {
            return db.Courses.ToList();
        }

        public static void Create(ApplicationDbContext db, Course course)
        {
            db.Courses.Add(course);
            db.SaveChanges();
        }

        public static void Edit(ApplicationDbContext db, Course course)
        {
            db.Courses.Attach(course);
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static void Delete(ApplicationDbContext db, int courseId)
        {
            var courseToDelete = db.Courses.Include(c => c.students).FirstOrDefault(c => c.Id == courseId);

            if (courseToDelete != null)
            {
                db.Courses.Remove(courseToDelete);

                foreach (var student in courseToDelete.students)
                {
                    student.courses.Remove(courseToDelete);
                }
            }

            db.SaveChanges();
        }

        public static void PrintInfo(ApplicationDbContext db, int courseId)
        {
            var course = db.Courses.Include(c => c.students).FirstOrDefault(c => c.Id == courseId);

            if (course != null)
            {
                Console.WriteLine($"Course ID: {course.Id}");
                Console.WriteLine($"Title: {course.Title}");
                Console.WriteLine($"Description: {course.Description}");

                if (course.students != null && course.students.Any())
                {
                    Console.WriteLine("Students enrolled:");
                    foreach (var student in course.students)
                    {
                        Console.WriteLine($"- {student.Name} | email: {student.Email}");
                    }
                }
                else
                    Console.WriteLine("No students enrolled.");
            }
        }

        //Sorting the courses by how many students have enrolled in them
        public static void BusiestCourses(ApplicationDbContext db)
        {
            //Step 1 - Order courses by the amount of students in them
            //Step 2 - Project the title of the course and the amount of students in it
            //Step 3 - materialize the query
            var busiestCourses = db.Courses.
                OrderByDescending(x => x.students.Count()).
                Select(x => new { x.Title, StudentsAmount = x.students.Count() }).
                ToList();

            //Print the list of courses we got from the query
            foreach (var course in busiestCourses)
            {
                Console.WriteLine($"{course.Title} - {course.StudentsAmount}");
            }
        }
    }
}
