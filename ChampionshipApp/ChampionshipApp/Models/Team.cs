namespace ChampionshipApp.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
