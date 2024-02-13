using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EF_Training_Student_Enrollment.Models
{
    public class Student
    {
        public Student()
        {
            this.courses = new HashSet<Course>();
        }
        public int Id { get; set; }
        
        [MaxLength(70)]
        public string? Name { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Course>? courses { get; set; }
    }
}
