using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class TeamDto
    {
        [Key]
        public string Id { get; set; } = default!;

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public virtual TeamTypeDto? TeamType { get; set; }

        //
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayersDto> Players { get; set; } = new HashSet<TeamPlayersDto>();

        public virtual ICollection<ChampionshipTeamsDto> Championships { get; set; } = new HashSet<ChampionshipTeamsDto>();
    }
}
