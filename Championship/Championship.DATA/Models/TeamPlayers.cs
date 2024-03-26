using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class TeamPlayers
    {
        [Key]
        public int Id { get; set; }

        public string? PlayerId { get; set; }
        public virtual Player? Player { get; set; } = default;

        public int? TeamId { get; set; }
        public virtual Team? Team { get; set; } = default;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;
    }
}
