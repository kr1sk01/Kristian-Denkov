using Championship.API.Models;
using Championship.DATA.Models;
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

        public void SeedData()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                SeedStaticTypes(context);
                SeedDynamicTypes(context);
            }
        }

        private static void SeedStaticTypes(ApplicationDbContext context)
        {
            SeedRoles(context);
            SeedTeamTypes(context);
            SeedGameTypes(context);
            SeedGameStatuses(context);
            SeedChampionshipTypes(context);
            SeedChampionshipStatuses(context);
        }

        private static void SeedDynamicTypes(ApplicationDbContext context)
        {
            SeedPlayers(context);
            SeedTeams(context);
            SeedGames(context);
            SeedChampionships(context);
            SeedTeamPlayers(context);
            SeedChampionshipTeams(context);
        }

        private static void SeedRoles(ApplicationDbContext context)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole { Id = "1", Name = "admin" },
                new IdentityRole { Id = "2", Name = "user" }
            };

            foreach (var role in roles)
            {
                if (!context.Roles.Contains(role))
                    context.Roles.Add(role);
            }

            context.SaveChanges();
        }

        private static void SeedTeamTypes(ApplicationDbContext context)
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
                if(!context.TeamTypes.Contains(teamType))
                    context.TeamTypes.Add(teamType);
            }

            context.SaveChanges();
        }

        private static void SeedGameTypes(ApplicationDbContext context)
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
                if(!context.GameTypes.Contains(gameType))
                    context.GameTypes.Add(gameType);
            }

            context.SaveChanges();
        }

        private static void SeedGameStatuses(ApplicationDbContext context)
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
                if (!context.GameStatuses.Contains(gameStatus))
                    context.GameStatuses.Add(gameStatus);
            }

            context.SaveChanges();
        }

        private static void SeedChampionshipTypes(ApplicationDbContext context)
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
                if (!context.ChampionshipTypes.Contains(championshipType))
                    context.ChampionshipTypes.Add(championshipType);
            }

            context.SaveChanges();
        }

        private static void SeedChampionshipStatuses(ApplicationDbContext context)
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
                if (!context.ChampionshipStatuses.Contains(championshipStatus))
                    context.ChampionshipStatuses.Add(championshipStatus);
            }

            context.SaveChanges();
        }

        private static void SeedPlayers(ApplicationDbContext context)
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
                if (!context.Users.Contains(player))
                    context.Users.Add(player);
            }

            var userRole = new IdentityUserRole<string> { RoleId = "1", UserId = "1" };
            if (!context.UserRoles.Contains(userRole))
                context.UserRoles.Add(userRole);

            context.SaveChanges();
        }

        private static void SeedTeams(ApplicationDbContext context)
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
                if (!context.Teams.Contains(team))
                    context.Teams.Add(team);    
            }

            context.SaveChanges();
        }

        private static void SeedGames(ApplicationDbContext context)
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
                if (!context.Games.Contains(game))
                    context.Games.Add(game);
            }

            context.SaveChanges();
        }

        private static void SeedChampionships(ApplicationDbContext context)
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
                if (!context.Championships.Contains(championship))
                    context.Championships.Add(championship);
            }

            context.SaveChanges();
        }

        private static void SeedTeamPlayers(ApplicationDbContext context)
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
                if (!context.TeamPlayers.Contains(teamPlayer))
                    context.TeamPlayers.Add(teamPlayer);
            }

            context.SaveChanges();
        }

        private static void SeedChampionshipTeams(ApplicationDbContext context)
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
                if (!context.ChampionshipTeams.Contains(champishipTeam))
                    context.ChampionshipTeams.Add(champishipTeam);
            }

            context.SaveChanges();
        }
    }
}
