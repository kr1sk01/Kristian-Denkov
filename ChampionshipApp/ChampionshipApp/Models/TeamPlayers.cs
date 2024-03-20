using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipApp.Models
{
    public class TeamPlayers
    {
        public TeamPlayers()
        {
            Players = new HashSet<Player>();
        }
        public int ID { get; set; }
        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public Team Team { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime? CreatedOn { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
