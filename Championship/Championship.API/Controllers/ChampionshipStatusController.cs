using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Championship.DATA.Models;
using Championship.SHARED.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipStatusController : ControllerBase
    {
        private readonly List<ChampionshipStatus> _championshipStatuses = new List<ChampionshipStatus>();

        // GET: api/ChampionshipStatus
        [HttpGet]
        public ActionResult<IEnumerable<ChampionshipStatus>> Get()
        {
            return Ok(_championshipStatuses);
        }

        // GET: api/ChampionshipStatus/5
        [HttpGet("{id}")]
        public ActionResult<ChampionshipStatus> Get(string id)
        {
            var championshipStatus = _championshipStatuses.FirstOrDefault(cs => cs.Id == id);
            if (championshipStatus == null)
            {
                return NotFound();
            }
            return Ok(championshipStatus);
        }

        // POST: api/ChampionshipStatus
        [HttpPost]
        public ActionResult<ChampionshipStatus> Post([FromBody] ChampionshipStatus championshipStatus)
        {
            _championshipStatuses.Add(championshipStatus);
            return CreatedAtAction(nameof(Get), new { id = championshipStatus.Id }, championshipStatus);
        }

        // PUT: api/ChampionshipStatus/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ChampionshipStatus championshipStatus)
        {
            var existingChampionshipStatus = _championshipStatuses.FirstOrDefault(cs => cs.Id == id);
            if (existingChampionshipStatus == null)
            {
                return NotFound();
            }
            existingChampionshipStatus.Name = championshipStatus.Name;
            return NoContent();
        }

        // DELETE: api/ChampionshipStatus/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var championshipStatus = _championshipStatuses.FirstOrDefault(cs => cs.Id == id);
            if (championshipStatus == null)
            {
                return NotFound();
            }
            _championshipStatuses.Remove(championshipStatus);
            return NoContent();
        }
    }
}
