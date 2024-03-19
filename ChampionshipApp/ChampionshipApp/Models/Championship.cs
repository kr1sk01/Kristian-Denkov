namespace ChampionshipApp.Models
{
    public class Championship
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
