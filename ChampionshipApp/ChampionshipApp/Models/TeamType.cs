using System.ComponentModel.DataAnnotations;

namespace ChampionshipApp.Models
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
    }
}
