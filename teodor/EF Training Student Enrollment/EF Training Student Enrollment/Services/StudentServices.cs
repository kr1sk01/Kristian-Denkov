using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EF_Training_Student_Enrollment.Models;

namespace EF_Training_Student_Enrollment.Services
{
    public static class StudentServices
    {
        //Get student by ID
        public static Student? Get(ApplicationDbContext db, int studentId)
        {
            return db.Students.FirstOrDefault(s => s.Id == studentId);
        }

        //Get all students
        public static List<Student> GetAll(ApplicationDbContext db)
        {
            return db.Students.ToList();
        }

        //Add a new student
        public static void Create(ApplicationDbContext db, Student student)
        {
            db.Students.Add(student);
            db.SaveChanges();
        }

        //Editing a student's information
        public static void Edit(ApplicationDbContext db, Student student)
        {
            //Attaching the modified student object so the context can track it for changes
            db.Students.Attach(student);
            //Marking the object as modified so we can update it in the database
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
        }

        //Removing a student by ID
        public static void Delete(ApplicationDbContext db, int studentId)
        {
            //Get the student by ID and eagerly load their related entities (the courses they've enrolled in)
            var studentToDelete = db.Students.Include(s => s.courses).FirstOrDefault(s => s.Id == studentId);

            if (studentToDelete != null)
            {
                //Remove the student
                db.Students.Remove(studentToDelete);

                //Remove the student from the courses they've enrolled in
                foreach (var course in studentToDelete.courses)
                {
                    course.students.Remove(studentToDelete);
                }
            }

            db.SaveChanges();
        }

        //Printing a student's information by ID
        public static void PrintInfo(ApplicationDbContext db, int studentId)
        {
            var student = db.Students.Include(s => s.courses).FirstOrDefault(s => s.Id == studentId);

            if (student != null)
            {
                //Print the student's personal information
                Console.WriteLine($"Student ID: {student.Id}");
                Console.WriteLine($"Name: {student.Name}");
                Console.WriteLine($"Date of birth: {student.DateOfBirth}");
                Console.WriteLine($"Email: {student.Email}");

                //Print the courses they've enrolled in
                if (student.courses != null && student.courses.Any())
                {
                    Console.WriteLine("Courses enrolled:");
                    foreach (var course in student.courses)
                    {
                        Console.WriteLine($"- {course.Title}");
                    }
                }
                else
                    Console.WriteLine("No courses enrolled.");
            }
        }

        //Enroll a student in a course
        public static void EnrollInCourse(ApplicationDbContext db, int studentId, int courseId)
        {
            var student = db.Students.FirstOrDefault(s => s.Id == studentId);
            var course = db.Courses.FirstOrDefault(c => c.Id == courseId);

            //If both the student ID and course ID are valid => Enroll the student
            if (student != null && course != null)
            {
                student.courses.Add(course);
                db.SaveChanges();
            }
            else
                Console.WriteLine("Student or course not found.");
        }

        //Withdraw a student from a course
        public static void WithdrawFromCourse(ApplicationDbContext db, int studentId, int courseId)
        {
            var student = db.Students.Include(s => s.courses).FirstOrDefault(s => s.Id == studentId);
            var course = db.Courses.FirstOrDefault(c => c.Id == courseId);

            //Check if both the student and course exist
            if (student != null && course != null)
            {
                //Check if the student has enrolled in the course
                //If they have => withdraw them from the course
                if (student.courses.Contains(course))
                {
                    student.courses.Remove(course);
                    course.students.Remove(student);
                    db.SaveChanges();
                }
                else
                    Console.WriteLine("The student hasn't enrolled in that course.");
                
            }
            else
                Console.WriteLine("Student or course not found.");
        }
    }
}
