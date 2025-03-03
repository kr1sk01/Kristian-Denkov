﻿namespace ChampionshipMaster.SHARED.DTO
{
    public class GameDto
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? GameTypeName { get; set; }

        public string? GameStatusName { get; set; }

        public TeamDto? BlueTeam { get; set; }

        public TeamDto? RedTeam { get; set; }

        public int? BluePoints { get; set; }

        public int? RedPoints { get; set; }

        public string? WinnerName { get; set; }

        public TeamDto? Winner { get; set; }

        public string? ChampionshipName { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedByUsername { get; set; }

        public int? MaxPoints { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? Date { get; set; }
    }
}
