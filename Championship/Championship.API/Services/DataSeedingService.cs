using Championship.API.Models;
using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Championship.API.Services
{
    public class DataSeedingService
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DataSeedingService(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public async Task SeedData()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                await SeedStaticTypes(context);
                await SeedDynamicTypes(context);
            }
        }

        private static async Task SeedStaticTypes(ApplicationDbContext context)
        {
            await SeedRoles(context);
            await SeedTeamTypes(context);
            await SeedGameTypes(context);
            await SeedGameStatuses(context);
            await SeedChampionshipTypes(context);
            await SeedChampionshipStatuses(context);
        }

        private static async Task SeedDynamicTypes(ApplicationDbContext context)
        {
            await SeedPlayers(context);
            await SeedTeams(context);
            await SeedGames(context);
            await SeedChampionships(context);
            await SeedTeamPlayers(context);
            await SeedChampionshipTeams(context);
        }

        private static async Task SeedRoles(ApplicationDbContext context)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole { Id = "1", Name = "admin" },
                new IdentityRole { Id = "2", Name = "user" }
            };

            foreach (var role in roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Id == role.Id))
                    await context.Roles.AddAsync(role);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeamTypes(ApplicationDbContext context)
        {
            List<TeamType> teamTypes = new List<TeamType>()
            {
                new TeamType 
                {   
                    Id = "1", 
                    Name = "Solo", 
                    TeamSize = 1 
                },
                new TeamType
                {
                    Id = "2",
                    Name = "Duo",
                    TeamSize = 2
                },
                new TeamType
                {
                    Id = "3",
                    Name = "Trio",
                    TeamSize = 3
                }
            };

            foreach (var teamType in teamTypes)
            {
                if(!await context.TeamTypes.AnyAsync(t => t.Id == teamType.Id))
                    await context.TeamTypes.AddAsync(teamType);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGameTypes(ApplicationDbContext context)
        {
            List<GameType> gameTypes = new List<GameType>()
            {
                new GameType 
                { 
                    Id = "1", 
                    Name = "Mortal Kombat", 
                    MaxPoints = 2, 
                    TeamTypeId = "1" 
                },
                new GameType 
                { 
                    Id = "2", 
                    Name = "Foosball", 
                    MaxPoints = 10, 
                    TeamTypeId = "2" 
                }
            };

            foreach(var gameType in gameTypes)
            {
                if(!await context.GameTypes.AnyAsync(g => g.Id == gameType.Id))
                    await context.GameTypes.AddAsync(gameType);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGameStatuses(ApplicationDbContext context)
        {
            List<GameStatus> gameStatuses = new List<GameStatus>()
            {
                new GameStatus 
                { 
                    Id = "1", 
                    Name = "Coming" 
                },
                new GameStatus 
                { 
                    Id = "2", 
                    Name = "Live" 
                },
                new GameStatus 
                { 
                    Id = "3", 
                    Name = "Finished" 
                },
                new GameStatus 
                { 
                    Id = "4", 
                    Name = "Cancelled" 
                }
            };

            foreach (var gameStatus in gameStatuses)
            {
                if (!await context.GameStatuses.AnyAsync(g => g.Id == gameStatus.Id))
                    await context.GameStatuses.AddAsync(gameStatus);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipTypes(ApplicationDbContext context)
        {
            List<ChampionshipType> championshipTypes = new List<ChampionshipType>()
            {
                new ChampionshipType 
                { 
                    Id = "1", 
                    Name = "Knockout" 
                },
                new ChampionshipType 
                { 
                    Id = "2", 
                    Name = "Extended" 
                }
            };

            foreach (var championshipType in championshipTypes)
            {
                if (!await context.ChampionshipTypes.AnyAsync(c => c.Id == championshipType.Id))
                    await context.ChampionshipTypes.AddAsync(championshipType);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipStatuses(ApplicationDbContext context)
        {
            List<ChampionshipStatus> championshipStatuses = new List<ChampionshipStatus>()
            {
                new ChampionshipStatus 
                { 
                    Id = "1", 
                    Name = "Coming" 
                },
                new ChampionshipStatus 
                { 
                    Id = "2", 
                    Name = "Live" 
                },
                new ChampionshipStatus 
                { 
                    Id = "3", 
                    Name = "Finished" 
                },
                new ChampionshipStatus 
                { 
                    Id = "4", 
                    Name = "Cancelled" 
                }
            };

            foreach (var championshipStatus in championshipStatuses)
            {
                if (!await context.ChampionshipStatuses.AnyAsync(c => c.Id == championshipStatus.Id))
                    await context.ChampionshipStatuses.AddAsync(championshipStatus);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedPlayers(ApplicationDbContext context)
        {
            List<Player> players = new List<Player>()
            {
                new Player 
                { 
                    Id = "1", 
                    UserName = "Pesho", 
                    Email = "Pesho@gmail.com", 
                    Active = true, 
                    AccessFailedCount = 0, 
                    EmailConfirmed = true, 
                    PhoneNumberConfirmed = false, 
                    LockoutEnabled = false, 
                    TwoFactorEnabled = false 
                },
                new Player 
                { 
                    Id = "2", 
                    UserName = "Gosho", 
                    Email = "Gosho@gmail.com", 
                    Active = true, 
                    AccessFailedCount = 0, 
                    EmailConfirmed = true, 
                    PhoneNumberConfirmed = false, 
                    LockoutEnabled = false, 
                    TwoFactorEnabled = false 
                },
                new Player 
                { 
                    Id = "3", 
                    UserName = "Ivan", 
                    Email = "Ivan@gmail.com", 
                    Active = true, 
                    AccessFailedCount = 0, 
                    EmailConfirmed = true, 
                    PhoneNumberConfirmed = false, 
                    LockoutEnabled = false, 
                    TwoFactorEnabled = false 
                },
                new Player 
                { 
                    Id = "4", 
                    UserName = "Asen", 
                    Email = "Asen@gmail.com", 
                    Active = true, 
                    AccessFailedCount = 0, 
                    EmailConfirmed = true, 
                    PhoneNumberConfirmed = false, 
                    LockoutEnabled = false, 
                    TwoFactorEnabled = false 
                }
            };

            foreach (var player in players)
            {
                if (!await context.Users.AnyAsync(p => p.Id == player.Id))
                    await context.Users.AddAsync(player);
            }

            var userRole = new IdentityUserRole<string> { RoleId = "1", UserId = "1" };
            if (!await context.UserRoles.AnyAsync(u => u.RoleId == userRole.RoleId && u.UserId == userRole.UserId))
                await context.UserRoles.AddAsync(userRole);

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeams(ApplicationDbContext context)
        {
            List<Team> teams = new List<Team>()
            {
                new Team 
                { 
                    Id = "1",
                    Name = "Team1", 
                    TeamTypeId = "2", 
                    Active = true 
                },
                new Team 
                { 
                    Id = "2", 
                    Name = "Team2", 
                    TeamTypeId = "2", 
                    Active = true 
                }
            };

            foreach (var team in teams)
            {
                if (!await context.Teams.AnyAsync(c => c.Id == team.Id))
                    await context.Teams.AddAsync(team);    
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGames(ApplicationDbContext context)
        {
            List<Game> games = new List<Game>()
            {
                new Game 
                { 
                    Id = "1", 
                    Name = "Game1", 
                    GameTypeId = "2", 
                    GameStatusId = "2", 
                    BlueTeamId = "1", 
                    RedTeamId = "2" 
                },
                new Game 
                { 
                    Id = "2", 
                    Name = "Game2", 
                    GameTypeId = "2", 
                    GameStatusId = "2", 
                    BlueTeamId = "2", 
                    RedTeamId = "1", 
                    WinnerId = "2", 
                    BluePoints = context.GameTypes.First(g => g.Id == "2").MaxPoints 
                }
            };

            foreach (var game in games)
            {
                if (!await context.Games.AnyAsync(g => g.Id == game.Id))
                    await context.Games.AddAsync(game);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionships(ApplicationDbContext context)
        {
            List<ChampionshipClass> championships = new List<ChampionshipClass>()
            {
                new ChampionshipClass 
                { 
                    Id = "1", 
                    Name = "Championship1", 
                    ChampionshipTypeId = "1", 
                    ChampionshipStatusId = "1", 
                    GameTypeId = "2", 
                    LotDate = DateTime.UtcNow 
                }
            };

            foreach (var championship in championships)
            {
                if (!await context.Championships.AnyAsync(c => c.Id == championship.Id))
                    await context.Championships.AddAsync(championship);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeamPlayers(ApplicationDbContext context)
        {
            List<TeamPlayers> teamPlayers = new List<TeamPlayers>()
            {
                new TeamPlayers 
                { 
                    Id = "1", 
                    PlayerId = "1", 
                    TeamId = "1" 
                },
                new TeamPlayers 
                { 
                    Id = "2", 
                    PlayerId = "2", 
                    TeamId = "1" 
                }
            };

            foreach (var teamPlayer in teamPlayers)
            {
                if (!await context.TeamPlayers.AnyAsync(t => t.Id == teamPlayer.Id || (t.PlayerId == teamPlayer.PlayerId && t.TeamId == teamPlayer.TeamId)))
                    await context.TeamPlayers.AddAsync(teamPlayer);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipTeams(ApplicationDbContext context)
        {
            List<ChampionshipTeams> championshipTeams = new List<ChampionshipTeams>()
            {
                new ChampionshipTeams 
                { 
                    Id = "1", 
                    ChampionshipId = "1", 
                    TeamId = "1" 
                }
            };

            foreach (var champishipTeam in championshipTeams)
            {
                if (!await context.ChampionshipTeams.AnyAsync(c => c.Id == champishipTeam.Id || (c.ChampionshipId == champishipTeam.ChampionshipId && c.TeamId == champishipTeam.TeamId)))
                    await context.ChampionshipTeams.AddAsync(champishipTeam);
            }

            await context.SaveChangesAsync();
        }
    }
}
