namespace ChampionshipMaster.SHARED.DTO
{
    public class ChampionshipTeamsDto
    {

        public int Id { get; set; }

        public int? ChampionshipId { get; set; }

        public int? TeamId { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
