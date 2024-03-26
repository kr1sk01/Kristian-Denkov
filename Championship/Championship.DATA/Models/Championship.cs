using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class Championship
    {
        [Key]
        public int Id { get; set; }

        //
        [StringLength(255)]
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public int? ChampionshipTypeId { get; set; }
        [ForeignKey("ChampionshipTypeId")]
        public virtual ChampionshipType? ChampionshipType { get; set; }

        public int? ChampionshipStatusId { get; set; }
        [ForeignKey("ChampionshipStatusId")]
        public virtual ChampionshipStatus? ChampionshipStatus { get; set; }

        public int? GameTypeId { get; set; }
        [ForeignKey("GameTypeId")]
        public virtual GameType? GameType { get; set; }

        public int? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public virtual Team? Winner { get; set; }

        public DateTime? LotDate { get; set; }

        public DateTime? Date { get; set; }

        //
        public string? CreatedBy { get; set; }

        //
        public DateTime CreatedOn { get; set; } = default!;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<Game> Games { get; set; } = new HashSet<Game>();

        public virtual ICollection<ChampionshipTeams> Teams { get; set; } = new HashSet<ChampionshipTeams>();
    }
}
