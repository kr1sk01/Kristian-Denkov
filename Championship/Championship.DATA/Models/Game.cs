using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Championship.DATA.Models
{
    public class Game
    {
        [Key]
        public string Id { get; set; } = default!;

        [StringLength(255)]
        public string? Name { get; set; }

        public string? GameTypeId { get; set; }
        [ForeignKey("GameTypeId")]
        public virtual GameType? GameType { get; set; }

        public string? GameStatusId { get; set; }
        [ForeignKey("GameStatusId")]
        public virtual GameStatus? GameStatus { get; set; }

        public string? BlueTeamId { get; set; }
        [ForeignKey("BlueTeamId")]
        public virtual Team? BlueTeam { get; set; }

        public string? RedTeamId { get; set; }
        [ForeignKey("RedTeamId")]
        public virtual Team? RedTeam { get; set; }

        public int? BluePoints { get; set; }

        public int? RedPoints { get; set; }

        public string? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public virtual Team? Winner { get; set; }

        public string? ChampionshipId { get; set; }
        [ForeignKey("ChampionshipId")]
        public virtual ChampionshipClass? Championship { get; set; }

        public DateTime? Date { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
