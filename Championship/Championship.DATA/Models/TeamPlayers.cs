using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class TeamPlayers
    {
        [Key]
        public string Id { get; set; } = default!;

        public string? PlayerId { get; set; }
        public virtual Player? Player { get; set; }

        public string? TeamId { get; set; }
        public virtual Team? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
