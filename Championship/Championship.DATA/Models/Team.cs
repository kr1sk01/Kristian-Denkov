using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[] Logo { get; set; } = default!;

        public int? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public virtual TeamType? TeamType { get; set; } = default;

        //
        public bool Active { get; set; } = default!;

        public string? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = default!;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayers> Players { get; set; } = new HashSet<TeamPlayers>();

        public virtual ICollection<ChampionshipTeams> Championships { get; set; } = new HashSet<ChampionshipTeams>();
    }
}
