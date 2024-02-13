using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_Training_Student_Enrollment.Models
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=True;Database=StudentEnrollment");          
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }
    }
}
