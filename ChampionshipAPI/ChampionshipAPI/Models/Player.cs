using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChampionshipAPI.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Avatar { get; set; } = default;

        public bool? Active { get; set; } = null;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayers> Teams { get; set; } = new HashSet<TeamPlayers>();
    }
}
