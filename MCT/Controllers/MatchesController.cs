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

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Matches
                .Include(m => m.MatchTypeNavigation)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches
                .Include(m => m.MatchTypeNavigation)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();

            return View(match);
        }

        public IActionResult Create()
        {
            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName");
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchId,TournamentId,TeamAId,TeamBId,ScheduledAt,ScoreA,ScoreB,MatchType")] Match match)
        {
            if (match.TeamAId.HasValue && match.TeamBId.HasValue && match.TeamAId == match.TeamBId)
            {
                ModelState.AddModelError("TeamBId", "A team cannot play against itself.");
            }

            if (match.TournamentId.HasValue && match.ScheduledAt.HasValue)
            {
                var tournament = await _context.Tournaments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TournamentId == match.TournamentId);

                if (tournament != null && tournament.StartDate.HasValue && tournament.EndDate.HasValue)
                {
                    var matchDate = match.ScheduledAt.Value.Date;
                    var startDate = tournament.StartDate.Value.Date;
                    var endDate = tournament.EndDate.Value.Date;

                    if (matchDate < startDate || matchDate > endDate)
                    {
                        ModelState.AddModelError("ScheduledAt", $"Date must be between {startDate:MM/dd/yyyy} and {endDate:MM/dd/yyyy}.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                int scoreA = match.ScoreA ?? 0;
                int scoreB = match.ScoreB ?? 0;

                if (scoreA > scoreB) match.WinnerId = match.TeamAId;
                else if (scoreB > scoreA) match.WinnerId = match.TeamBId;
                else match.WinnerId = null;

                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            return View(match);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound();

            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MatchId,TournamentId,TeamAId,TeamBId,ScheduledAt,ScoreA,ScoreB,MatchType")] Match match)
        {
            if (id != match.MatchId) return NotFound();

            if (match.TeamAId.HasValue && match.TeamBId.HasValue && match.TeamAId == match.TeamBId)
            {
                ModelState.AddModelError("TeamBId", "A team cannot play against itself.");
            }

            if (match.TournamentId.HasValue && match.ScheduledAt.HasValue)
            {
                var tournament = await _context.Tournaments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TournamentId == match.TournamentId);

                if (tournament != null && tournament.StartDate.HasValue && tournament.EndDate.HasValue)
                {
                    var matchDate = match.ScheduledAt.Value.Date;
                    var startDate = tournament.StartDate.Value.Date;
                    var endDate = tournament.EndDate.Value.Date;

                    if (matchDate < startDate || matchDate > endDate)
                    {
                        ModelState.AddModelError("ScheduledAt", $"Date must be between {startDate:MM/dd/yyyy} and {endDate:MM/dd/yyyy}.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int scoreA = match.ScoreA ?? 0;
                    int scoreB = match.ScoreB ?? 0;

                    if (scoreA > scoreB) match.WinnerId = match.TeamAId;
                    else if (scoreB > scoreA) match.WinnerId = match.TeamBId;
                    else match.WinnerId = null;

                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.MatchId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MatchType"] = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            ViewData["TeamA"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewData["TeamB"] = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewData["Tournament"] = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            return View(match);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches
                .Include(m => m.MatchTypeNavigation)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Include(m => m.Winner)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();

            return View(match);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null) _context.Matches.Remove(match);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
            return _context.Matches.Any(e => e.MatchId == id);
        }
    }
}