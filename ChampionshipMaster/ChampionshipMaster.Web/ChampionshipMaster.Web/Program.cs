using Blazored.LocalStorage;
using Blazorise;
using ChampionshipMaster.Web.Components;
using ChampionshipMaster.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;
using Radzen.Blazor;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace ChampionshipMaster.Web;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddRadzenComponents();


        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddTransient<INotifier, Notifier>();
        builder.Services.AddTransient<IImageService, ImageService>();
        builder.Services.AddTransient<IClientLotService, ClientLotService>();
        builder.Services.AddScoped<LayoutStateService>();
        builder.Services.AddHttpClient(builder.Configuration["ClientName"]!, client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"]!);
        });

        builder.Services.AddSignalR();
        builder.Services.AddBlazoredLocalStorage();


        builder.Services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.Run();
    }
}
