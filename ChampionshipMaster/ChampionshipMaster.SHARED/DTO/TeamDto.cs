namespace ChampionshipMaster.SHARED.DTO
{
    public class TeamDto
    {
        public override string? ToString()
        {
            return Name;
        }
        public int Id { get; set; }

        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? TeamTypeName { get; set; }

        public int TeamSize { get; set; }

        public bool? Active { get; set; } = true;

        public string? CreatedBy { get; set; }
        public string? CreatedByUsername { get; set; }

        public DateTime? CreatedOn { get; set; }

        public virtual ICollection<PlayerDto> Players { get; set; } = new HashSet<PlayerDto>();

        //public virtual ICollection<ChampionshipDto> Championships { get; set; } = new HashSet<ChampionshipDto>();
    }
}
