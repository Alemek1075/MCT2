using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly MctContext _context;

        public LeaderboardController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams
                .Include(t => t.MatchTeamAs)
                .Include(t => t.MatchTeamBs)
                .ToListAsync();

            var rankings = teams.Select(t => {
                int won = t.MatchTeamAs.Sum(m => m.ScoreA ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreB ?? 0);
                int lost = t.MatchTeamAs.Sum(m => m.ScoreB ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreA ?? 0);
                int total = won + lost;
                int matchesPlayed = t.MatchTeamAs.Count + t.MatchTeamBs.Count;

                return new LeaderboardViewModel
                {
                    TeamId = t.TeamId,
                    TeamName = t.Name ?? "TBD",
                    ShortCode = t.ShortCode ?? t.Name ?? "TBD",
                    WonRounds = won,
                    LostRounds = lost,
                    Diff = won - lost,
                    WinPercentage = total > 0 ? (double)won / total * 100 : 0,
                    MatchesPlayed = matchesPlayed
                };
            })
            .Where(x => x.MatchesPlayed >= 3)
            .OrderByDescending(x => x.WinPercentage)
            .ThenByDescending(x => x.Diff)
            .ToList();

            return View(rankings);
        }
    }
}