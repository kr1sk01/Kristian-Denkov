using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.API.Services.ControllerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ChampionshipMaster.API.Services
{
    public static class ControllerServicesRegistering
    {
        public static void RegisterControllerServices(this IServiceCollection services)
        {
            services.AddTransient<IChampionshipService, ChampionshipService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IGameStatusService, GameStatusService>();
            services.AddTransient<IGameTypeService, GameTypeService>();
            services.AddTransient<IChampionshipStatusService, ChampionshipStatusService>();
            services.AddTransient<IChampionshipTeamsService, ChampionshipTeamsService>();
            services.AddTransient<IChampionshipTypeService, ChampionshipTypeService>();
            services.AddTransient<ITeamPlayersService, TeamPlayersService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<ITeamTypesService, TeamTypesService>();
            services.AddHttpContextAccessor();
            services.AddTransient<UserManager<Player>>();
            services.AddTransient<SignInManager<Player>>();
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<JwtService>();
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
