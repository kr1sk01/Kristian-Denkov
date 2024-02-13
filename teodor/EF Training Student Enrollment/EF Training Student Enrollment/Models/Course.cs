using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EF_Training_Student_Enrollment.Models
{
    public class Course
    {
        public Course()
        {
            this.students = new HashSet<Student>();
        }
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Title {  get; set; }

        public string? Description { get; set; }

        public virtual ICollection<Student>? students { get; set; }
    }
}
