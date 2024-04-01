using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Mapster;

namespace Championship.API.Data
{
    public static class DtoMapping
    {
        public static void ConfigureMappings()
        {
            // ChampionshipClass mapping
            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.ChampionshipStatusName, src => src.ChampionshipStatus.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.ChampionshipTypeName, src => src.ChampionshipType.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.GameTypeName, src => src.GameType.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.WinnerName, src => src.Winner.Name);

            // Game mapping
            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.GameTypeName, src => src.GameType.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.GameStatusName, src => src.GameStatus.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.BlueTeamName, src => src.BlueTeam.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.RedTeamName, src => src.RedTeam.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.WinnerName, src => src.Winner.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
              .Map(dest => dest.ChampionshipName, src => src.Championship.Name);

            // Team mapping
            TypeAdapterConfig<Team, TeamDto>.NewConfig()
                .Map(dest => dest.TeamTypeName, src => src.TeamType.Name);

            TypeAdapterConfig<Team, TeamDto>.NewConfig()
                .Map(dest => dest.Players, src => src.Players);

            // Player mapping
            TypeAdapterConfig<Player, PlayerDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id);

            TypeAdapterConfig<Player, PlayerDto>.NewConfig()
                .Map(dest => dest.Name, src => src.UserName);

            // TeamPlayers mapping
            TypeAdapterConfig<TeamPlayers, TeamPlayersDto>.NewConfig()
                .Map(dest => dest.PlayerName, src => src.Player.UserName);

            TypeAdapterConfig<TeamPlayers, TeamPlayersDto>.NewConfig()
                .Map(dest => dest.TeamName, src => src.Team.Name);

            TypeAdapterConfig<TeamPlayers, PlayerDto>.NewConfig()
                .Map(dest => dest.Id, src => src.PlayerId);

            TypeAdapterConfig<TeamPlayers, PlayerDto>.NewConfig()
                .Map(dest => dest.Name, src => src.Player.UserName);

            TypeAdapterConfig<TeamPlayers, PlayerDto>.NewConfig()
                .Map(dest => dest.Active, src => src.Player.Active);

            
        }
    }
}
