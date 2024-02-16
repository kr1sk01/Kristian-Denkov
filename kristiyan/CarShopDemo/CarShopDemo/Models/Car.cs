using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShopDemo.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int ProductionYear { get; set; }
        [Required]
        public int EngineVolume { get; set; }
        [Required]
        public int HorsePower { get; set; }
        public string? Description { get; set; }
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
        public int ColorsId { get; set; }
        public virtual ICollection<Color> Colors { get; set; }

    }
}
