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

            var rankings = teams.Select(t => new LeaderboardViewModel
            {
                TeamId = t.TeamId,
                TeamName = t.Name ?? "TBD",
                ShortCode = t.ShortCode ?? t.Name ?? "TBD",
                WonRounds = t.MatchTeamAs.Sum(m => m.ScoreA ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreB ?? 0),
                LostRounds = t.MatchTeamAs.Sum(m => m.ScoreB ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreA ?? 0),
                Diff = (t.MatchTeamAs.Sum(m => m.ScoreA ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreB ?? 0)) -
                       (t.MatchTeamAs.Sum(m => m.ScoreB ?? 0) + t.MatchTeamBs.Sum(m => m.ScoreA ?? 0))
            })
            .OrderByDescending(x => x.Diff)
            .ToList();

            return View(rankings);
        }
    }
}