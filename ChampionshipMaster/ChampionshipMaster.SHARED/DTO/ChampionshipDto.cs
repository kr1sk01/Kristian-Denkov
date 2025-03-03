﻿namespace ChampionshipMaster.SHARED.DTO
{
    public class ChampionshipDto
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public byte[]? Logo { get; set; }
        public string? ChampionshipTypeName { get; set; }
        public string? ChampionshipStatusName { get; set; }
        public GameTypeDto? GameType { get; set; }
        public string? WinnerName { get; set; }
        public byte[]? WinnerLogo { get; set; }
        public DateTime? LotDate { get; set; }
        public DateTime? Date { get; set; }
        public virtual ICollection<GameDto> Games { get; set; } = new HashSet<GameDto>();
        public virtual ICollection<TeamDto> Teams { get; set; } = new HashSet<TeamDto>();
    }
}
