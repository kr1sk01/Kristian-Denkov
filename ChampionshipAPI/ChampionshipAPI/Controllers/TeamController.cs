using ChampionshipAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChampionshipAPI.Controllers
{
    public static class TeamController
    {
        public static async Task<IResult> GetTeams(HttpContext http, ChampionshipContext context)
        {
            var teams = await context.Teams.ToListAsync();
            return Results.Ok(teams);
        }

        public static async Task<IResult> GetTeam(HttpContext http, ChampionshipContext context, int id)
        {
            var team = await context.Teams.FindAsync(id);

            if (team == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(team);
        }

        public static async Task<IResult> AddTeam(HttpContext http, ChampionshipContext context, Team team)
        {
            team.CreatedOn = DateTime.UtcNow;
            await context.Teams.AddAsync(team);
            await context.SaveChangesAsync();
            return Results.Created($"/teams/{team.Id}", team);
        }

        public static async Task<IResult> EditTeam(HttpContext http, ChampionshipContext context, int id, Team inputTeam)
        {
            var team = await context.Teams.FindAsync(id);
            if (team == null)
            {
                return Results.NotFound();
            }

            team.Name = inputTeam.Name;
            team.Logo = inputTeam.Logo;
            team.TeamType = inputTeam.TeamType;
            team.Active = inputTeam.Active;
            team.CreatedBy = inputTeam.CreatedBy;
            team.CreatedOn = inputTeam.CreatedOn;
            team.ModifiedOn = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        public static async Task<IResult> DeleteTeam(HttpContext http, ChampionshipContext context, int id)
        {
            var team = await context.Teams.FindAsync(id);

            if (team == null)
            {
                return Results.NotFound();
            }

            context.Teams.Remove(team);
            await context.SaveChangesAsync();
            return Results.Ok(team);
        }
    }
}
