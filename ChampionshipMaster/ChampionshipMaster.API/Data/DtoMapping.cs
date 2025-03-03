﻿using System.Reflection;

namespace ChampionshipMaster.API.Data
{
    public static class DtoMapping
    {
        private static readonly ApplicationDbContext _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        public static void ConfigureMappings(this IServiceCollection services)
        {
            TypeAdapterConfig<Championship, ChampionshipDto>.NewConfig()
                .Map(dest => dest.Teams, src => src.ChampionshipTeams.Select(sc => sc.Team).Adapt<List<TeamDto>>())
                .Map(dest => dest.WinnerLogo, src => src.Winner == null ? null : src.Winner.Logo)
                .Map(dest => dest.GameType, src => src.GameType.Adapt<GameTypeDto>());

            TypeAdapterConfig<Team, TeamDto>.NewConfig()
                .Map(dest => dest.Players, src => src.TeamPlayers.Select(sc => sc.Player).Adapt<List<PlayerDto>>())
                .Map(dest => dest.TeamSize, src => src.TeamType!.TeamSize);
                //.Map(dest => dest.Championships, src => src.ChampionshipTeams.Select(x => x.Championship));
                //.Map(dest => dest.CreatedByUsername, src => _context.Users.FirstOrDefault(x => x.Id == src.CreatedBy)!.UserName);


            TypeAdapterConfig<Player, PlayerDto>.NewConfig()
                .Map(dest => dest.Name, src => src.UserName);

            TypeAdapterConfig<TeamDto, Team>.NewConfig()
                .Map(dest => dest.TeamType, src => _context.TeamTypes.FirstOrDefault(x => x.Name == src.TeamTypeName));

            TypeAdapterConfig<GameType, GameTypeDto>.NewConfig()
                .Map(dest => dest.TeamTypeName, src => src.TeamType!.Name);

            TypeAdapterConfig<Game, GameDto>.NewConfig()
                .Map(dest => dest.MaxPoints, src => src.GameType!.MaxPoints)
                .Map(dest => dest.BlueTeam, src => src.BlueTeam.Adapt<TeamDto>())
                .Map(dest => dest.RedTeam, src => src.RedTeam.Adapt<TeamDto>());

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}