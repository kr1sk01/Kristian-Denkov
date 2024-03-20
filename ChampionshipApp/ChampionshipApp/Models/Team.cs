using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipApp.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        public byte[] Logo { get; set; } = default!;
        [ForeignKey("TeamType")]
        public int TeamTypeId { get; set; }
        public TeamType TeamType { get; set; } = default!;
        public bool Active { get; set; } = default!;
        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public TeamPlayers TeamPlayers { get; set; } = default!;
    }
}
