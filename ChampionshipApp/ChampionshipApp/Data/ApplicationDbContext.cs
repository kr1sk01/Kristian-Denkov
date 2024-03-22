using ChampionshipApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ChampionshipApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<TeamType> TeamTypes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<GameStatus> GameStatuses { get; set; }
        public DbSet<Championship> Championships { get; set; }
        public DbSet<ChampionshipType> ChampionshipTypes { get; set; }
        public DbSet<ChampionshipStatus> ChampionshipStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(p => p.Teams)
                .WithMany(t => t.Players)
                .UsingEntity<TeamPlayers>();
        }
    }
}
