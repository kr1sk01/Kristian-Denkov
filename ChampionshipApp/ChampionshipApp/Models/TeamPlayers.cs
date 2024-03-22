using ChampionshipApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipApp.Models
{
    public class TeamPlayers
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public ApplicationUser Player { get; set; }
        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public Team Team { get; set; }

        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
