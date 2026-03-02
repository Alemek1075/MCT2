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
    public class TournamentStatusController : Controller
    {
        private readonly MctContext _context;

        public TournamentStatusController(MctContext context)
        {
            _context = context;
        }

        // GET: TournamentStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.TournamentStatuses.ToListAsync());
        }

        // GET: TournamentStatus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentStatus = await _context.TournamentStatuses
                .FirstOrDefaultAsync(m => m.StatusName == id);
            if (tournamentStatus == null)
            {
                return NotFound();
            }

            return View(tournamentStatus);
        }

        // GET: TournamentStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TournamentStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] TournamentStatus tournamentStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournamentStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentStatus);
        }

        // GET: TournamentStatus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentStatus = await _context.TournamentStatuses.FindAsync(id);
            if (tournamentStatus == null)
            {
                return NotFound();
            }
            return View(tournamentStatus);
        }

        // POST: TournamentStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StatusName")] TournamentStatus tournamentStatus)
        {
            if (id != tournamentStatus.StatusName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournamentStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentStatusExists(tournamentStatus.StatusName))
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
            return View(tournamentStatus);
        }

        // GET: TournamentStatus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentStatus = await _context.TournamentStatuses
                .FirstOrDefaultAsync(m => m.StatusName == id);
            if (tournamentStatus == null)
            {
                return NotFound();
            }

            return View(tournamentStatus);
        }

        // POST: TournamentStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tournamentStatus = await _context.TournamentStatuses.FindAsync(id);
            if (tournamentStatus != null)
            {
                _context.TournamentStatuses.Remove(tournamentStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentStatusExists(string id)
        {
            return _context.TournamentStatuses.Any(e => e.StatusName == id);
        }
    }
}
