using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Mapster;

namespace Championship.API.Data
{
    public static class DtoMapping
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.ChampionshipStatusName, src => src.ChampionshipStatus.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.ChampionshipTypeName, src => src.ChampionshipType.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.GameTypeName, src => src.GameType.Name);

            TypeAdapterConfig<ChampionshipClass, ChampionshipClassDto>.NewConfig()
              .Map(dest => dest.WinnerName, src => src.Winner.Name);

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
        }
    }
}
