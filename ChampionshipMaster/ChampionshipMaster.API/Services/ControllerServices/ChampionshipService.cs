using ChampionshipMaster.API.Services.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipService : IChampionshipService
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChampionshipDto>> GetAllChampionships()
        {
            var championships = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .ToListAsync();

            var dto = championships.Adapt<List<ChampionshipDto>>();
            return dto;
        }

        public async Task<List<ChampionshipDto>> GetChampionshipsDetails()
        {
            var championshipClassesWithDetails = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .Include(x => x.Games)
                .ThenInclude(g => g.GameStatus)
            .Include(x => x.Games)
                .ThenInclude(g => g.RedTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.BlueTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.Winner).ToListAsync();

            var dtos = championshipClassesWithDetails.Adapt<List<ChampionshipDto>>();
            return dtos;
        }

        public async Task<Championship?> GetChampionship(int id)
        {
            var championshipClass = await _context.Championships
            .Include(x => x.ChampionshipStatus)
            .Include(x => x.ChampionshipType)
            .Include(x => x.Winner)
            .Include(x => x.GameType)
            .Include(x => x.Games)
                .ThenInclude(g => g.GameStatus)
            .Include(x => x.Games)
                .ThenInclude(g => g.RedTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.BlueTeam)
            .Include(x => x.Games)
                .ThenInclude(g => g.Winner)
            .Include(x => x.ChampionshipTeams)
                .ThenInclude(x => x.Team)
            .FirstOrDefaultAsync(a => a.Id == id);

            return championshipClass;
        }

        public async Task PostChampionship(Championship championship)
        {
            _context.Championships.Add(championship);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChampionshipNameExists(string name)
        {
            return await _context.Championships.AnyAsync(x => x.Name == name);
        }
    }
}
