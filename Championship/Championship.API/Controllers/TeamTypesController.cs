﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;

namespace Championship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamType>>> GetTeamTypes()
        {
            return await _context.TeamTypes.ToListAsync();
        }

        // GET: api/TeamTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamType>> GetTeamType(string id)
        {
            var teamType = await _context.TeamTypes.FindAsync(id);

            if (teamType == null)
            {
                return NotFound();
            }

            return teamType;
        }

        // PUT: api/TeamTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamType(string id, TeamType teamType)
        {
            if (id != teamType.Id)
            {
                return BadRequest();
            }

            _context.Entry(teamType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamTypeExists(id))
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

        // POST: api/TeamTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamType>> PostTeamType(TeamType teamType)
        {
            _context.TeamTypes.Add(teamType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamTypeExists(teamType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeamType", new { id = teamType.Id }, teamType);
        }

        // DELETE: api/TeamTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamType(string id)
        {
            var teamType = await _context.TeamTypes.FindAsync(id);
            if (teamType == null)
            {
                return NotFound();
            }

            _context.TeamTypes.Remove(teamType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamTypeExists(string id)
        {
            return _context.TeamTypes.Any(e => e.Id == id);
        }
    }
}
