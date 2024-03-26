using System.ComponentModel.DataAnnotations;

namespace Championship.DATA.Models
{
    public class ChampionshipTeams
    {
        [Key]
        public string Id { get; set; } = default!;

        public string? ChampionshipId { get; set; }
        public virtual ChampionshipClass? Championship { get; set; }

        public string? TeamId { get; set; }
        public virtual Team? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
