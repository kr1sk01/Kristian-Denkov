using Championship.API.Models;
using Championship.DATA.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Championship.API.Services
{
    public class DataSeeding
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DataSeeding(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
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

        public async Task SeedTeamTypes()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                string jsonData = await File.ReadAllTextAsync("Services/JsonDataSeeds/TeamTypes.json");
                List<TeamType>? dataList = Deserialize<TeamType>(jsonData);

                foreach (var item in dataList)
                {
                    if (!await context.TeamTypes.AnyAsync(x => x.Id == item.Id))
                        await context.TeamTypes.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
