using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipApp.Models
{
    public class GameType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        [Required]
        public int MaxPoints { get; set; }
        [Required]
        [ForeignKey("TeamType")]
        public int TeamTypeId { get; set; }
        public TeamType TeamType { get; set; }
    }
}
