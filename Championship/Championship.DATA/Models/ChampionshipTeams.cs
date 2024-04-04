using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class ChampionshipTeams
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ChampionshipId { get; set; }
        public virtual ChampionshipClass? Championship { get; set; }

        public int? TeamId { get; set; }
        public virtual Team? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
