using Championship.DATA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            SeedData(builder);

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


            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            SeedStaticTypes(builder);
            SeedDynamicTypes(builder);
            SeedTeamPlayers(builder);
            SeedChampionshipTeams(builder);
        }

        private void SeedStaticTypes(ModelBuilder builder)
        {
            SeedTeamTypes(builder);
            SeedGameTypes(builder);
            SeedGameStatuses(builder);
            SeedChampionshipTypes(builder);
            SeedChampionshipStatuses(builder);
        }

        private void SeedDynamicTypes(ModelBuilder builder)
        {
            SeedPlayers(builder);
            SeedTeams(builder);
            SeedGames(builder);
            SeedChampionships(builder);
        }

        private void SeedTeamTypes(ModelBuilder builder)
        {
            builder.Entity<TeamType>().HasData(
                new TeamType { Id = "1", Name = "Solo", TeamSize = 1 },
                new TeamType { Id = "2", Name = "Duo", TeamSize = 2 },
                new TeamType { Id = "3", Name = "Trio", TeamSize = 3 }
            );
        }

        private void SeedGameTypes(ModelBuilder builder)
        {
            builder.Entity<GameType>().HasData(
                new GameType { Id = "1", Name = "Mortal Kombat", MaxPoints = 2, TeamTypeId = "1" },
                new GameType { Id = "2", Name = "Foosball", MaxPoints = 10, TeamTypeId = "2" }
            );
        }

        private void SeedGameStatuses(ModelBuilder builder)
        {
            builder.Entity<GameStatus>().HasData(
                new GameStatus { Id = "1", Name = "Coming" },
                new GameStatus { Id = "2", Name = "Live" },
                new GameStatus { Id = "3", Name = "Finished" },
                new GameStatus { Id = "4", Name = "Cancelled" }
            );
        }

        private void SeedChampionshipTypes(ModelBuilder builder)
        {
            builder.Entity<ChampionshipType>().HasData(
                new ChampionshipType { Id = "1", Name = "Knockout" },
                new ChampionshipType { Id = "2", Name = "Extended" }
            );
        }

        private void SeedChampionshipStatuses(ModelBuilder builder)
        {
            builder.Entity<ChampionshipStatus>().HasData(
                new ChampionshipStatus { Id = "1", Name = "Coming" },
                new ChampionshipStatus { Id = "2", Name = "Live" },
                new ChampionshipStatus { Id = "3", Name = "Finished" },
                new ChampionshipStatus { Id = "4", Name = "Cancelled" }
            );
        }

        private void SeedPlayers(ModelBuilder builder)
        {
            builder.Entity<Player>().HasData(
                new Player { Id = "1", UserName = "Pesho", Email = "Pesho@gmail.com", Active = true, AccessFailedCount = 0, EmailConfirmed = true, PhoneNumberConfirmed = false, LockoutEnabled = false, TwoFactorEnabled = false },
                new Player { Id = "2", UserName = "Gosho", Email = "Gosho@gmail.com", Active = true, AccessFailedCount = 0, EmailConfirmed = true, PhoneNumberConfirmed = false, LockoutEnabled = false, TwoFactorEnabled = false },
                new Player { Id = "3", UserName = "Ivan", Email = "Ivan@gmail.com", Active = true, AccessFailedCount = 0, EmailConfirmed = true, PhoneNumberConfirmed = false, LockoutEnabled = false, TwoFactorEnabled = false },
                new Player { Id = "4", UserName = "Asen", Email = "Asen@gmail.com", Active = true, AccessFailedCount = 0, EmailConfirmed = true, PhoneNumberConfirmed = false, LockoutEnabled = false, TwoFactorEnabled = false }
            );
        }

        private void SeedTeams(ModelBuilder builder)
        {
            builder.Entity<Team>().HasData(
                new Team { Id = "1", Name = "Team1", TeamTypeId = "2", Active = true },
                new Team { Id = "2", Name = "Team2", TeamTypeId = "2", Active = true }
            );
        }

        private void SeedGames(ModelBuilder builder)
        {
            builder.Entity<Game>().HasData(
                new Game { Id = "1", Name = "Game1", GameTypeId = "2", GameStatusId = "2", BlueTeamId = "1", RedTeamId = "2" },
                new Game { Id = "2", Name = "Game2", GameTypeId = "2", GameStatusId = "2", BlueTeamId = "2", RedTeamId = "1",  WinnerId = "2", BluePoints = GameTypes.First(g => g.Id == "2").MaxPoints }
            );
        }

        private void SeedChampionships(ModelBuilder builder)
        {
            builder.Entity<ChampionshipClass>().HasData(
                new ChampionshipClass { Id = "1", Name = "Championship1", ChampionshipTypeId = "1", ChampionshipStatusId = "1", GameTypeId = "2", LotDate = DateTime.UtcNow }
            );
        }

        private void SeedTeamPlayers(ModelBuilder builder)
        {
            builder.Entity<TeamPlayers>().HasData(
                new TeamPlayers { Id = "1", PlayerId = "1", TeamId = "1"},
                new TeamPlayers { Id = "2", PlayerId = "2", TeamId = "1" }
            );
        }

        private void SeedChampionshipTeams(ModelBuilder builder)
        {
            builder.Entity<ChampionshipTeams>().HasData(
                new ChampionshipTeams { Id = "1", ChampionshipId = "1", TeamId = "1" }
            );
        }

    }
}
