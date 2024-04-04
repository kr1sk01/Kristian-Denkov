namespace ChampionshipMaster.SHARED.DTO
{
    public class TeamPlayersDto
    {

        public int Id { get; set; }

        public string? PlayerName { get; set; }
        public virtual PlayerDto? Player { get; set; }

        public string? TeamName { get; set; }
        public virtual TeamDto? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
