using System.Reflection;

namespace ChampionshipMaster.API.Data
{
    public static class DtoMapping
    {
        public static void ConfigureMappings(this IServiceCollection services)
        {
            TypeAdapterConfig<Championship, ChampionshipDto>.NewConfig()
                .Map(dest => dest.Teams, src => src.ChampionshipTeams.Select(sc => sc.Team).Adapt<List<TeamDto>>());

            TypeAdapterConfig<Team, TeamDto>.NewConfig()
                .Map(dest => dest.Players, src => src.TeamPlayers.Select(sc => sc.Player).Adapt<List<PlayerDto>>());

            TypeAdapterConfig<Player, PlayerDto>.NewConfig()
                .Map(dest => dest.Name, src => src.UserName);

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
