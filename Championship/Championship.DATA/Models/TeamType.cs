using System.ComponentModel.DataAnnotations;

namespace Championship.DATA.Models
{
    public class TeamType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        [Required]
        public int TeamSize { get; set; }

        public virtual ICollection<Team> Teams { get; set;} = new HashSet<Team>();
    }
}
