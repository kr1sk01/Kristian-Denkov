
using ChampionshipAPI.Controllers;
using ChampionshipAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChampionshipAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ChampionshipContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpClient("ChampionshipAPI", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://example.com");
            });
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/teams", (HttpContext http, ChampionshipContext context) => TeamController.GetTeams(http, context));
            app.MapGet("/team/{id}", (HttpContext http, ChampionshipContext context, int id) => TeamController.GetTeam(http, context, id));
            app.MapPost("/team", (HttpContext http, ChampionshipContext context, Team team) => TeamController.AddTeam(http, context, team));
            app.MapPut("/team/{id}", (HttpContext http, ChampionshipContext context, int id, Team team) => TeamController.EditTeam(http, context, id, team));
            app.MapDelete("/team/{id}", (HttpContext http, ChampionshipContext context, int id) => TeamController.DeleteTeam(http, context, id));

            app.Run();
        }
    }
}
