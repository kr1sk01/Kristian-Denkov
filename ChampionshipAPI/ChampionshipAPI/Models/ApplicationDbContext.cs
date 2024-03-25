using Microsoft.EntityFrameworkCore;

namespace ChampionshipAPI.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayers> TeamPlayers { get; set; }
        public DbSet<TeamType> TeamTypes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameType> GameTypes { get; set; }
        public DbSet<GameStatus> GameStatuses { get; set; }
        public DbSet<Championship> Championships { get; set; }
        public DbSet<ChampionshipType> ChampionshipTypes { get; set; }
        public DbSet<ChampionshipStatus> ChampionshipStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeamPlayers>()
                .HasOne(pt => pt.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(pt => pt.TeamId);

            modelBuilder.Entity<TeamPlayers>()
                .HasOne(pt => pt.Player)
                .WithMany(t => t.Teams)
                .HasForeignKey(pt => pt.PlayerId);

            modelBuilder.Entity<ChampionshipTeams>()
                .HasOne(pt => pt.Championship)
                .WithMany(t => t.Teams)
                .HasForeignKey(pt => pt.ChampionshipId);

            modelBuilder.Entity<ChampionshipTeams>()
                .HasOne(pt => pt.Team)
                .WithMany(t => t.Championships)
                .HasForeignKey(pt => pt.TeamId);
        }
    }
}
