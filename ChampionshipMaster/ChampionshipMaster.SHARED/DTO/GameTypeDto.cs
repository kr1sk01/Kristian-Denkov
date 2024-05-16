namespace ChampionshipMaster.SHARED.DTO
{
    public class GameTypeDto
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public int? MaxPoints { get; set; }

        public TeamDto? TeamTypeName { get; set; }
    }
}
