using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var matches = _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB);
            return View(await matches.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();

            return View(match);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Match matchForm)
        {
            if (id != matchForm.MatchId) return NotFound();

            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound();

            match.ScoreA = matchForm.ScoreA;
            match.ScoreB = matchForm.ScoreB;

            if (match.ScoreA > match.ScoreB) match.WinnerId = match.TeamAId;
            else if (match.ScoreB > match.ScoreA) match.WinnerId = match.TeamBId;
            else match.WinnerId = null;

            try
            {
                _context.Update(match);
                await _context.SaveChangesAsync();

                await CheckAndGenerateNextRound(match.TournamentId);
            }
            catch (DbUpdateConcurrencyException) { throw; }

            return RedirectToAction(nameof(Index));
        }

        private async Task CheckAndGenerateNextRound(int? tournamentId)
        {
            if (tournamentId == null) return;

            var allMatches = await _context.Matches
                .Where(m => m.TournamentId == tournamentId)
                .OrderBy(m => m.ScheduledAt)
                .ToListAsync();

            if (!allMatches.Any()) return;

            var currentRoundDate = allMatches.Max(m => m.ScheduledAt.Value.Date);
            var currentRoundMatches = allMatches.Where(m => m.ScheduledAt.Value.Date == currentRoundDate).ToList();

            bool allCompleted = currentRoundMatches.All(m => m.WinnerId != null);

            if (currentRoundMatches.Count <= 1) return;

            if (allCompleted)
            {
                var winners = currentRoundMatches.OrderBy(m => m.ScheduledAt).Select(m => m.WinnerId.Value).ToList();

                DateTime nextMatchTime = currentRoundDate.AddDays(1).AddHours(10);

                for (int i = 0; i < winners.Count; i += 2)
                {
                    if (i + 1 < winners.Count)
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
                        nextMatchTime = nextMatchTime.AddHours(2);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}