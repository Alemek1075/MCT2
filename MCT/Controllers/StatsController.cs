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
    public class StatsController : Controller
    {
        private readonly MctContext _context;

        public StatsController(MctContext context)
        {
            _context = context;
        }

        // GET: Stats
        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Stats.Include(s => s.Match).Include(s => s.Player);
            return View(await mctContext.ToListAsync());
        }

        // GET: Stats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats
                .Include(s => s.Match)
                .Include(s => s.Player)
                .FirstOrDefaultAsync(m => m.StatId == id);
            if (stat == null)
            {
                return NotFound();
            }

            return View(stat);
        }

        // GET: Stats/Create
        public IActionResult Create()
        {
            ViewData["MatchId"] = new SelectList(_context.Matches, "MatchId", "MatchId");
            ViewData["PlayerId"] = new SelectList(_context.Players, "PlayerId", "PlayerId");
            return View();
        }

        // POST: Stats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatId,PlayerId,MatchId,Kills,Deaths,Assists,HsPercentage")] Stat stat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "MatchId", "MatchId", stat.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "PlayerId", "PlayerId", stat.PlayerId);
            return View(stat);
        }

        // GET: Stats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats.FindAsync(id);
            if (stat == null)
            {
                return NotFound();
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "MatchId", "MatchId", stat.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "PlayerId", "PlayerId", stat.PlayerId);
            return View(stat);
        }

        // POST: Stats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatId,PlayerId,MatchId,Kills,Deaths,Assists,HsPercentage")] Stat stat)
        {
            if (id != stat.StatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatExists(stat.StatId))
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
            ViewData["MatchId"] = new SelectList(_context.Matches, "MatchId", "MatchId", stat.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "PlayerId", "PlayerId", stat.PlayerId);
            return View(stat);
        }

        // GET: Stats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stat = await _context.Stats
                .Include(s => s.Match)
                .Include(s => s.Player)
                .FirstOrDefaultAsync(m => m.StatId == id);
            if (stat == null)
            {
                return NotFound();
            }

            return View(stat);
        }

        // POST: Stats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stat = await _context.Stats.FindAsync(id);
            if (stat != null)
            {
                _context.Stats.Remove(stat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatExists(int id)
        {
            return _context.Stats.Any(e => e.StatId == id);
        }
    }
}
