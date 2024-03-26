using Championship.DATA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Championship.API.Models
{
    public class ApplicationDbContext : IdentityDbContext<Player>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TeamPlayers>()
               .HasOne(p => p.Team)
               .WithMany(t => t.Players)
               .HasForeignKey(p => p.TeamId);

            builder.Entity<TeamPlayers>()
               .HasOne(p => p.Player)
               .WithMany(t => t.Teams)
               .HasForeignKey(p => p.PlayerId);

            builder.Entity<ChampionshipTeams>()
               .HasOne(p => p.Team)
               .WithMany(t => t.Championships)
               .HasForeignKey(p => p.TeamId);

            builder.Entity<ChampionshipTeams>()
               .HasOne(p => p.Championship)
               .WithMany(t => t.Teams)
               .HasForeignKey(p => p.ChampionshipId);
        }
    }
}
