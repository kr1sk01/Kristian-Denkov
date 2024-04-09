using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.API.Services.ControllerServices;
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
        }
    }
}
