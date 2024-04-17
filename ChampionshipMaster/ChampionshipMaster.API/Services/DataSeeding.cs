using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace ChampionshipMaster.API.Services
{
    public class DataSeeding
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly UserManager<Player> _userManager;

        public DataSeeding(DbContextOptions<ApplicationDbContext> options, UserManager<Player> userManager)
        {
            _options = options;
            _userManager = userManager;
        }

        public static List<T>? Deserialize<T>(string jsonData)
        {
            using (var jsonDocument = JsonDocument.Parse(jsonData))
            {
                var rootElement = jsonDocument.RootElement;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return rootElement.Deserialize<List<T>>(options);
            }
        }

        public async Task SeedData()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                await SeedTeamTypes(context);
                await SeedGameTypes(context);
                await SeedGameStatuses(context);
                await SeedChampionshipTypes(context);
                await SeedChampionshipStatuses(context);
                await SeedRoles(context);
                await SeedPlayers(context);
                await SeedUserRoles(context);
                await SeedTeams(context);
                await SeedChampionships(context);
                await SeedGames(context);
                await SeedTeamPlayers(context);
                await SeedChampionshipTeams(context);
                await SeedAdminAccount();
            }
        }

        private static async Task SeedTeamTypes(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/TeamTypes.json");
            List<TeamType>? dataList = Deserialize<TeamType>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.TeamTypes.AnyAsync(x => x.Id == item.Id))
                        await context.TeamTypes.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGameTypes(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/GameTypes.json");
            List<GameType>? dataList = Deserialize<GameType>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.GameTypes.AnyAsync(x => x.Id == item.Id))
                        await context.GameTypes.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGameStatuses(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/GameStatuses.json");
            List<GameStatus>? dataList = Deserialize<GameStatus>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.GameStatuses.AnyAsync(x => x.Id == item.Id))
                        await context.GameStatuses.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipTypes(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/ChampionshipTypes.json");
            List<ChampionshipType>? dataList = Deserialize<ChampionshipType>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.ChampionshipTypes.AnyAsync(x => x.Id == item.Id))
                        await context.ChampionshipTypes.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipStatuses(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/ChampionshipStatuses.json");
            List<ChampionshipStatus>? dataList = Deserialize<ChampionshipStatus>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.ChampionshipStatuses.AnyAsync(x => x.Id == item.Id))
                        await context.ChampionshipStatuses.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/Roles.json");
            List<IdentityRole>? dataList = Deserialize<IdentityRole>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.Roles.AnyAsync(x => x.Id == item.Id))
                        await context.Roles.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedPlayers(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/Players.json");
            List<Player>? dataList = Deserialize<Player>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.Users.AnyAsync(x => x.Id == item.Id))
                        await context.Users.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedUserRoles(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/UserRoles.json");
            List<IdentityUserRole<string>>? dataList = Deserialize<IdentityUserRole<string>>(jsonData);

            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.UserRoles.AnyAsync(x => x.UserId == item.UserId && x.RoleId == item.RoleId))
                        await context.UserRoles.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeams(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/Teams.json");
            List<Team>? dataList = Deserialize<Team>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.Teams.AnyAsync(x => x.Id == item.Id))
                        await context.Teams.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedGames(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/Games.json");
            List<Game>? dataList = Deserialize<Game>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.Games.AnyAsync(x => x.Id == item.Id))
                        await context.Games.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionships(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/Championships.json");
            List<Championship>? dataList = Deserialize<Championship>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.Championships.AnyAsync(x => x.Id == item.Id))
                        await context.Championships.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeamPlayers(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/TeamPlayers.json");
            List<TeamPlayers>? dataList = Deserialize<TeamPlayers>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.TeamPlayers.AnyAsync(x => x.Id == item.Id))
                        await context.TeamPlayers.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private static async Task SeedChampionshipTeams(ApplicationDbContext context)
        {
            string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/ChampionshipTeams.json");
            List<ChampionshipTeams>? dataList = Deserialize<ChampionshipTeams>(jsonData);
            if (dataList != null)
                foreach (var item in dataList)
                {
                    if (!await context.ChampionshipTeams.AnyAsync(x => x.Id == item.Id))
                        await context.ChampionshipTeams.AddAsync(item);
                }

            await context.SaveChangesAsync();
        }

        private async Task SeedAdminAccount()
        {
            var UserName = "admin";
            var Email = "admin@admin.com";
            var Password = "admin4dm1n";

            var admin = await _userManager.FindByNameAsync(UserName);
            if (admin == null)
            {
                await _userManager.CreateAsync(new Player { UserName = UserName, Email = Email }, Password);
                var createdAdmin = await _userManager.FindByNameAsync(UserName);
                await _userManager.AddToRoleAsync(createdAdmin!, "admin");

                return;
            }
            else
            {
                var hasAdminRole = await _userManager.IsInRoleAsync(admin!, "admin");
                if (!hasAdminRole)
                {
                    await _userManager.AddToRoleAsync(admin!, "admin");
                }
            }
        }
    }
}
