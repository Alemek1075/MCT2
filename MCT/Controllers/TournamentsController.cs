using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly MctContext _context;

        public TournamentsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Tournaments.Include(t => t.StatusNavigation);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        public IActionResult Create()
        {
            ViewData["Status"] = new SelectList(_context.TournamentStatuses, "StatusName", "StatusName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TournamentId,Description,Location,StartDate,EndDate,Price,Status")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(_context.TournamentStatuses, "StatusName", "StatusName", tournament.Status);
            return View(tournament);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            ViewData["Status"] = new SelectList(_context.TournamentStatuses, "StatusName", "StatusName", tournament.Status);
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TournamentId,Description,Location,StartDate,EndDate,Price,Status")] Tournament tournament)
        {
            if (id != tournament.TournamentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.TournamentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(_context.TournamentStatuses, "StatusName", "StatusName", tournament.Status);
            return View(tournament);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null) _context.Tournaments.Remove(tournament);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.TournamentId == id);
        }
    }
}