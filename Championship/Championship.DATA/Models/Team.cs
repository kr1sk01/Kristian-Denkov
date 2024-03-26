using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class Team
    {
        [Key]
        public string Id { get; set; } = default!;

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public virtual TeamType? TeamType { get; set; }

        //
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayers> Players { get; set; } = new HashSet<TeamPlayers>();

        public virtual ICollection<ChampionshipTeams> Championships { get; set; } = new HashSet<ChampionshipTeams>();
    }
}
