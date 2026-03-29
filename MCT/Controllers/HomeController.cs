using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class HomeController : Controller
    {
        private readonly MctContext _context;

        public HomeController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _context.Matches
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .Include(m => m.Tournament)
                .Where(m => m.Tournament != null && m.Tournament.Status != "Canceled")
                .OrderBy(m => m.ScheduledAt)
                .ToListAsync();

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
                    TeamName = t.ShortCode ?? t.Name ?? "TBD",
                    ShortCode = t.ShortCode ?? "TBD",
                    WonRounds = won,
                    LostRounds = lost,
                    Diff = won - lost,
                    WinPercentage = total > 0 ? (double)won / total * 100 : 0,
                    MatchesPlayed = matchesPlayed,
                    MatchTeamAs = t.MatchTeamAs.ToList(),
                    MatchTeamBs = t.MatchTeamBs.ToList()
                };
            })
            .Where(x => x.MatchesPlayed >= 3)
            .OrderByDescending(x => x.WinPercentage)
            .ThenByDescending(x => x.Diff)
            .ToList();

            ViewBag.Rankings = rankings;

            return View(matches);
        }
    }
}