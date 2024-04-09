using ChampionshipMaster.API.Interfaces;

namespace ChampionshipMaster.API.Services.ControllerServices
{
    public class ChampionshipTypeService : ControllerBase, IChampionshipTypeService
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChampionshipTypeExists(int id)
        {
            return await _context.ChampionshipTypes.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> ChampionshipTypeNameExists(string? name)
        {
            return await _context.ChampionshipTypes.AnyAsync(t => t.Name == name);
        }

        public async Task<IActionResult> DeleteChampionshipType(int id)
        {
            var championshipType = await _context.ChampionshipTypes.FirstOrDefaultAsync(cs => cs.Id == id);

            if (championshipType == null)
            {
                return NotFound();
            }

            var championship = await _context.Championships.Where(x => x.ChampionshipTypeId == id).ToListAsync();

            foreach (var item in championship)
            {
                item.ChampionshipTypeId = null;

                _context.Entry(item).State = EntityState.Modified;
            }

            _context.ChampionshipTypes.Remove(championshipType);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> EditChampionshipType(int id, ChampionshipType championshipType)
        {
            if (id != championshipType.Id)
            {
                return BadRequest();
            }

            _context.Entry(championshipType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChampionshipTypeExists(id))
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

        public async Task<List<ChampionshipType>> GetAllChampionshipTypes()
        {
            return await _context.ChampionshipTypes.ToListAsync();
        }

        public async Task<ActionResult<ChampionshipType?>> GetChampionshipType(int id)
        {
            var championshipType = await _context.ChampionshipTypes.FirstOrDefaultAsync(cs => cs.Id == id);
            if (championshipType == null)
            {
                return NotFound();
            }
            return Ok(championshipType);
        }

        public async Task<ActionResult<ChampionshipType>> PostChampionshipType(ChampionshipType championshipType)
        {
            if (await ChampionshipTypeNameExists(championshipType.Name))
            {
                return BadRequest("There is already a championship type with that name");
            }

            await _context.ChampionshipTypes.AddAsync(championshipType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostChampionshipType), new
            {
                id = championshipType.Id
            },
                championshipType);
        }
    }
}
