using System.ComponentModel.DataAnnotations;

namespace ChampionshipAPI.Models
{
    public class GameStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
