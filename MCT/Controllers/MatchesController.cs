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
            return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).FirstOrDefaultAsync(m => m.MatchId == id));
        }

        public IActionResult Create()
        {
            ViewBag.TournamentId = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewBag.TeamAId = new SelectList(_context.Teams, "TeamId", "Name");
            ViewBag.TeamBId = new SelectList(_context.Teams, "TeamId", "Name");
            ViewBag.MatchType = new SelectList(_context.MatchTypes, "TypeName", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatchId,TournamentId,TeamAId,TeamBId,ScheduledAt,MatchType")] Match match)
        {
            if (match.TeamAId == match.TeamBId) ModelState.AddModelError("TeamBId", "Team B cannot be the same as Team A.");

            ModelState.Remove("TeamA"); ModelState.Remove("TeamB"); ModelState.Remove("Tournament");
            ModelState.Remove("Winner"); ModelState.Remove("MatchTypeNavigation"); ModelState.Remove("Stats");
            ModelState.Remove("ScoreA"); ModelState.Remove("ScoreB");

            if (ModelState.IsValid)
            {
                match.ScoreA = 0; match.ScoreB = 0;
                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TournamentId = new SelectList(_context.Tournaments, "TournamentId", "Description", match.TournamentId);
            ViewBag.TeamAId = new SelectList(_context.Teams, "TeamId", "Name", match.TeamAId);
            ViewBag.TeamBId = new SelectList(_context.Teams, "TeamId", "Name", match.TeamBId);
            ViewBag.MatchType = new SelectList(_context.MatchTypes, "TypeName", "TypeName", match.MatchType);
            return View(match);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).FirstOrDefaultAsync(m => m.MatchId == id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Match matchForm)
        {
            var match = await _context.Matches.Include(m => m.TeamA).Include(m => m.TeamB).FirstOrDefaultAsync(m => m.MatchId == id);
            if (match == null) return NotFound();

            if (match.TeamAId == null || match.TeamBId == null)
            {
                TempData["ErrorMessage"] = "Cannot update score: Both teams must be determined to play this match.";
                return RedirectToAction(nameof(Index));
            }

            match.ScoreA = matchForm.ScoreA;
            match.ScoreB = matchForm.ScoreB;

            if (match.ScoreA > match.ScoreB) match.WinnerId = match.TeamAId;
            else if (match.ScoreB > match.ScoreA) match.WinnerId = match.TeamBId;
            else match.WinnerId = null;

            try
            {
                _context.Update(match);
                await _context.SaveChangesAsync();

                await RecalculateBracket(match.TournamentId);
            }
            catch (DbUpdateConcurrencyException) { throw; }

            return RedirectToAction(nameof(Index));
        }

        private async Task RecalculateBracket(int? tournamentId)
        {
            if (tournamentId == null) return;

            var allMatches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId)
                .OrderBy(m => m.ScheduledAt)
                .ThenBy(m => m.MatchId)
                .ToListAsync();

            var rounds = allMatches.GroupBy(m => m.ScheduledAt.Value.Date).OrderBy(g => g.Key).ToList();

            for (int r = 0; r < rounds.Count - 1; r++)
            {
                var currentRound = rounds[r].ToList();
                var nextRound = rounds[r + 1].ToList();

                for (int i = 0; i < currentRound.Count; i++)
                {
                    int nextMatchIndex = i / 2;
                    bool isTeamA = i % 2 == 0;

                    if (nextMatchIndex < nextRound.Count)
                    {
                        var nextMatch = nextRound[nextMatchIndex];
                        int? currentWinner = currentRound[i].WinnerId;

                        int? targetTeamId = isTeamA ? nextMatch.TeamAId : nextMatch.TeamBId;

                        if (targetTeamId != currentWinner)
                        {
                            if (isTeamA) nextMatch.TeamAId = currentWinner;
                            else nextMatch.TeamBId = currentWinner;

                            nextMatch.ScoreA = 0;
                            nextMatch.ScoreB = 0;
                            nextMatch.WinnerId = null;

                            var stats = await _context.Stats.Where(s => s.MatchId == nextMatch.MatchId).ToListAsync();
                            if (stats.Any()) _context.Stats.RemoveRange(stats);

                            _context.Update(nextMatch);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).FirstOrDefaultAsync(m => m.MatchId == id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                if (await _context.Stats.AnyAsync(s => s.MatchId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete match: It has player statistics recorded. Delete stats first.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}