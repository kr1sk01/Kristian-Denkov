using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Championship.API.Models;
using Championship.DATA.Models;

namespace Championship.API.Controllers
{
    public class ChampionshipTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChampionshipTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChampionshipTypes.ToListAsync());
        }

        // GET: ChampionshipTypes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var championshipType = await _context.ChampionshipTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (championshipType == null)
            {
                return NotFound();
            }

            return View(championshipType);
        }

        // GET: ChampionshipTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChampionshipTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ChampionshipType championshipType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(championshipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(championshipType);
        }

        // GET: ChampionshipTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var championshipType = await _context.ChampionshipTypes.FindAsync(id);
            if (championshipType == null)
            {
                return NotFound();
            }
            return View(championshipType);
        }

        // POST: ChampionshipTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] ChampionshipType championshipType)
        {
            if (id != championshipType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(championshipType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChampionshipTypeExists(championshipType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(championshipType);
        }

        // GET: ChampionshipTypes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var championshipType = await _context.ChampionshipTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (championshipType == null)
            {
                return NotFound();
            }

            return View(championshipType);
        }

        // POST: ChampionshipTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var championshipType = await _context.ChampionshipTypes.FindAsync(id);
            if (championshipType != null)
            {
                _context.ChampionshipTypes.Remove(championshipType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChampionshipTypeExists(string id)
        {
            return _context.ChampionshipTypes.Any(e => e.Id == id);
        }
    }
}
