using ChampionshipMaster.API.Services.Interfaces;
using ChampionshipMaster.API.Services.Interfaces.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ChampionshipMaster.API.Services
{
    public static class ControllerServicesRegistering
    {
        public static void RegisterControllerServices(this IServiceCollection services)
        {
            services.AddTransient<IChampionshipService, ChampionshipService>();
        }
    }
}
