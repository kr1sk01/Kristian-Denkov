using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChampionshipAPI.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        public int? GameTypeID { get; set; }
        [ForeignKey("GameTypeID")]
        public virtual GameType? GameType { get; set; }

        public int? GameStatusID { get; set; }
        [ForeignKey("GameStatusID")]
        public virtual GameStatus? GameStatus { get; set; }

        public int? BlueTeamID { get; set; }
        [ForeignKey("BlueTeamID")]
        public virtual Team? BlueTeam { get; set; }

        public int? RedTeamID { get; set; }
        [ForeignKey("RedTeamID")]
        public virtual Team? RedTeam { get; set; }

        public int? BluePoints { get; set; }

        public int? RedPoints { get; set; }

        public int? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public virtual Team? Winner { get; set; }

        public int? ChampionshipId { get; set; }
        [ForeignKey("ChampionshipId")]
        public virtual Championship? Championship { get; set; }

        public DateTime? Date { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = null;
    }
}
