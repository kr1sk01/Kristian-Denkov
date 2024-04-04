namespace ChampionshipMaster.SHARED.DTO
{
    public class ChampionshipTeamsDto
    {

        public int Id { get; set; }

        public string? ChampionshipId { get; set; }
        public virtual ChampionshipDto? Championship { get; set; }

        public string? TeamId { get; set; }
        public virtual TeamDto? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
