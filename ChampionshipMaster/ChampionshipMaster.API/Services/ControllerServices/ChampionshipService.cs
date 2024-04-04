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
    }
}
