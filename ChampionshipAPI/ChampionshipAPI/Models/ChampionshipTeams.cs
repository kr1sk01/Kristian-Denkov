using System.ComponentModel.DataAnnotations;

namespace ChampionshipAPI.Models
{
    public class ChampionshipTeams
    {
        [Key]
        public int Id { get; set; }

        public int? ChampionshipId { get; set; }
        public virtual Championship? Championship { get; set; } = default;

        public int? TeamId { get; set; }
        public virtual Team? Team { get; set; } = default;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;
    }
}
