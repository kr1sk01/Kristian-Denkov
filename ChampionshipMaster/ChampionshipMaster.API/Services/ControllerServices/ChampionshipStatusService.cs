using ChampionshipMaster.API.Interfaces;
using ChampionshipMaster.DATA.Models;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipStatusService : ControllerBase, IChampionshipStatusService
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipStatusService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChampionshipStatusExists(int id)
        {
            return await _context.ChampionshipStatuses.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ChampionshipStatusNameExists(string? name)
        {
            return await _context.ChampionshipStatuses.AnyAsync(x => x.Name == name);
        }

        public async Task<IActionResult> DeleteChampionshipStatus(int id)
        {
            var championshipStatus = await _context.ChampionshipStatuses.FirstOrDefaultAsync(cs => cs.Id == id);

            if (championshipStatus == null)
            {
                return NotFound();
            }

            var championship = await _context.Championships.Where(x => x.ChampionshipStatusId == id).ToListAsync();

            foreach (var item in championship)
            {
                item.ChampionshipStatusId = null;

                _context.Entry(item).State = EntityState.Modified;
            }

            _context.ChampionshipStatuses.Remove(championshipStatus);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditChampionshipStatus(int id, ChampionshipStatus championshipStatus)
        {
            if (id != championshipStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(championshipStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChampionshipStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        public async Task<List<ChampionshipStatus>> GetAllChampionshipStatuses()
        {
            return await _context.ChampionshipStatuses.ToListAsync();
        }

        public async Task<ActionResult<ChampionshipStatus?>> GetChampionshipStatus(int id)
        {
            var championshipStatus = await _context.ChampionshipStatuses.FirstOrDefaultAsync(cs => cs.Id == id);
            if (championshipStatus == null)
            {
                return NotFound();
            }
            return Ok(championshipStatus);
        }

        public async Task<ActionResult<ChampionshipStatus>> PostChampionshipStatus(ChampionshipStatus championshipStatus)
        {
            if (await ChampionshipStatusNameExists(championshipStatus.Name))
            {
                return BadRequest("There is already a championship status with that name");
            }

            await _context.ChampionshipStatuses.AddAsync(championshipStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostChampionshipStatus), new
            {
                id = championshipStatus.Id
            },
                championshipStatus);
        }
    }
}
