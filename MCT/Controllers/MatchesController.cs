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
    public class MatchesController : Controller
    {
        private readonly MctContext _context;

        public MatchesController(MctContext context)
        {
            _context = context;
        }

        // GET: Matches
        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Matches.Include(m => m.MatchTypeNavigation).Include(m => m.TeamA).Include(m => m.TeamB).Include(m => m.Tournament).Include(m => m.Winner);
            return View(await mctContext.ToListAsync());
        }

        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.MatchTypeNavigation)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner)
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // GET: Matches/Create
        public IActionResult Create()
        {
            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName");
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewData["Winner"] = new SelectList(_context.Teams, "TeamId", "Name");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchId,TournamentId,TeamAId,TeamBId,WinnerId,ScheduledAt,ScoreA,ScoreB,MatchType")] Match match)
        {
            if (ModelState.IsValid)
            {
                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            ViewData["Winner"] = new SelectList(_context.Teams, "TeamId", "Name", match.WinnerId);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            ViewData["Winner"] = new SelectList(_context.Teams, "TeamId", "Name", match.WinnerId);
            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchId,TournamentId,TeamAId,TeamBId,WinnerId,ScheduledAt,ScoreA,ScoreB,MatchType")] Match match)
        {
            if (id != match.MatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.MatchId))
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
            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            ViewData["Winner"] = new SelectList(_context.Teams, "TeamId", "Name", match.WinnerId);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var match = await _context.Matches
                .Include(m => m.MatchTypeNavigation)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner)
                .FirstOrDefaultAsync(m => m.MatchId == id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return _context.Matches.Any(e => e.MatchId == id);
        }
    }
}
