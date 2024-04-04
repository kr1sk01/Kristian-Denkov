using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipMaster.DATA.Models
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public int? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public virtual TeamType? TeamType { get; set; }

        //
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayers> TeamPlayers { get; set; } = new HashSet<TeamPlayers>();

        public virtual ICollection<ChampionshipTeams> ChampionshipTeams { get; set; } = new HashSet<ChampionshipTeams>();
    }
}
