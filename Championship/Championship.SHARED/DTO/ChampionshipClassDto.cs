using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class ChampionshipClassDto
    {
        [Key]
        public string Id { get; set; } = default!;

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? ChampionshipTypeId { get; set; }
        [ForeignKey("ChampionshipTypeId")]
        public virtual ChampionshipTypeDto? ChampionshipType { get; set; }

        public string? ChampionshipStatusId { get; set; }
        [ForeignKey("ChampionshipStatusId")]
        public virtual ChampionshipStatusDto? ChampionshipStatus { get; set; }

        public string? GameTypeId { get; set; }
        [ForeignKey("GameTypeId")]
        public virtual GameTypeDto? GameType { get; set; }

        public string? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public virtual TeamDto? Winner { get; set; }

        public DateTime? LotDate { get; set; }

        public DateTime? Date { get; set; }

        //
        public string? CreatedBy { get; set; }

        //
        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<GameDto> Games { get; set; } = new HashSet<GameDto>();

        public virtual ICollection<ChampionshipTeamsDto> Teams { get; set; } = new HashSet<ChampionshipTeamsDto>();
    }
}
