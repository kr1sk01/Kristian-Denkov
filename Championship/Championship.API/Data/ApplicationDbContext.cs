using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

#pragma warning disable VSSpell001 // Spell Check
namespace Championship.API.Models
#pragma warning restore VSSpell001 // Spell Check
{
    public class ApplicationDbContext : IdentityDbContext<Player>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<TeamType> TeamTypes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<GameStatus> GameStatuses { get; set; }
        public DbSet<ChampionshipClass> Championships { get; set; }
        public DbSet<ChampionshipType> ChampionshipTypes { get; set; }
        public DbSet<ChampionshipStatus> ChampionshipStatuses { get; set; }
        public DbSet<ChampionshipTeams> ChampionshipTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TeamPlayers>()
                .HasOne(sc => sc.Player)
                .WithMany(s => s.TeamPlayers)
                .HasForeignKey(sc => sc.PlayerId);

            builder.Entity<TeamPlayers>()
                .HasOne(sc => sc.Team)
                .WithMany(c => c.TeamPlayers)
                .HasForeignKey(sc => sc.TeamId);

            builder.Entity<ChampionshipTeams>()
               .HasOne(p => p.Team)
               .WithMany(t => t.ChampionshipTeams)
               .HasForeignKey(p => p.TeamId);

            builder.Entity<ChampionshipTeams>()
               .HasOne(p => p.Championship)
               .WithMany(t => t.ChampionshipTeams)
               .HasForeignKey(p => p.ChampionshipId);
        }
    }
}
