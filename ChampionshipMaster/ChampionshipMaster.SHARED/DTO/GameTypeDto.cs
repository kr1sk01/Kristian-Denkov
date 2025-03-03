﻿namespace ChampionshipMaster.SHARED.DTO
{
    public class GameTypeDto
    {
        public override string? ToString()
        {
            return Name;
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? MaxPoints { get; set; }

        public string? TeamTypeName { get; set; }
    }
}
