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
        public MatchesController(MctContext context) { _context = context; }

        public async Task<IActionResult> Index() { return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).ToListAsync()); }
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); return View(await _context.Matches.Include(m => m.Tournament).Include(m => m.TeamA).Include(m => m.TeamB).FirstOrDefaultAsync(m => m.MatchId == id)); }

        // --- СТВОРЕННЯ МАТЧУ РУКАМИ ---
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

        // --- РЕДАГУВАННЯ ТА ЛОГІКА СІТКИ ---
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

            int? previousWinnerId = match.WinnerId;
            match.ScoreA = matchForm.ScoreA;
            match.ScoreB = matchForm.ScoreB;

            if (match.ScoreA > match.ScoreB) match.WinnerId = match.TeamAId;
            else if (match.ScoreB > match.ScoreA) match.WinnerId = match.TeamBId;
            else match.WinnerId = null;

            // ФІКС: Обриваємо наступні матчі тільки якщо рахунок змінився/збросився
            if (previousWinnerId != null && match.WinnerId != previousWinnerId)
            {
                await DeleteFutureMatches(match.TournamentId, previousWinnerId, match.ScheduledAt);
            }

            try
            {
                _context.Update(match);
                await _context.SaveChangesAsync();

                if (match.WinnerId != null) await CheckAndGenerateNextRound(match.TournamentId);
            }
            catch (DbUpdateConcurrencyException) { throw; }

            return RedirectToAction(nameof(Index));
        }

        // ФІКС: Шукаємо тільки матчі, які по графіку ПІЗНІШЕ (m.ScheduledAt > afterDate), щоб уникнути нескінченного циклу!
        private async Task DeleteFutureMatches(int? tournamentId, int? teamId, DateTime? afterDate)
        {
            if (tournamentId == null || teamId == null || afterDate == null) return;

            var futureMatches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId &&
                            (m.TeamAId == teamId || m.TeamBId == teamId) &&
                            m.ScheduledAt > afterDate)
                .ToListAsync();

            foreach (var fMatch in futureMatches)
            {
                int? nextWinnerId = fMatch.WinnerId;
                DateTime? nextDate = fMatch.ScheduledAt;

                var stats = await _context.Stats.Where(s => s.MatchId == fMatch.MatchId).ToListAsync();
                _context.Stats.RemoveRange(stats);
                _context.Matches.Remove(fMatch);

                if (nextWinnerId != null) await DeleteFutureMatches(tournamentId, nextWinnerId, nextDate);
            }
        }

        private async Task CheckAndGenerateNextRound(int? tournamentId)
        {
            if (tournamentId == null) return;
            var allMatches = await _context.Matches.Where(m => m.TournamentId == tournamentId).OrderBy(m => m.ScheduledAt).ToListAsync();
            if (!allMatches.Any()) return;

            var currentRoundDate = allMatches.Max(m => m.ScheduledAt.Value.Date);
            var currentRoundMatches = allMatches.Where(m => m.ScheduledAt.Value.Date == currentRoundDate).ToList();

            if (currentRoundMatches.Count <= 1) return; // Це фінал

            if (currentRoundMatches.All(m => m.WinnerId != null))
            {
                var winners = currentRoundMatches.OrderBy(m => m.ScheduledAt).Select(m => m.WinnerId.Value).ToList();
                DateTime nextMatchTime = currentRoundDate.AddDays(1).AddHours(10);

                for (int i = 0; i < winners.Count; i += 2)
                {
                    if (i + 1 < winners.Count)
                    {
                        bool exists = await _context.Matches.AnyAsync(m => m.TournamentId == tournamentId && m.TeamAId == winners[i] && m.TeamBId == winners[i + 1]);
                        if (!exists)
                        {
                            _context.Matches.Add(new Match
                            {
                                TournamentId = tournamentId,
                                TeamAId = winners[i],
                                TeamBId = winners[i + 1],
                                ScoreA = 0,
                                ScoreB = 0,
                                ScheduledAt = nextMatchTime,
                                MatchType = "Auto"
                            });
                        }
                        nextMatchTime = nextMatchTime.AddHours(2);
                    }
                }
                await _context.SaveChangesAsync();
            }
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