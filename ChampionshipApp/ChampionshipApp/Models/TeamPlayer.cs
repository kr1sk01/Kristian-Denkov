namespace ChampionshipApp.Models
{
    public class TeamPlayer
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public Player Player { get; set; }
        public int TeamID { get; set; }
        public Team Team { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
