using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly MctContext _context;

        public TournamentsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Tournaments.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                    .ThenInclude(m => m.TeamA)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.TeamB)
                .Include(t => t.TournamentTeams)
                    .ThenInclude(tt => tt.Team)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Teams = await _context.Teams.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tournament tournament)
        {
            ModelState.Remove("EndDate");
            ModelState.Remove("Status");

            int teamCount = tournament.SelectedTeamIds?.Count ?? 0;

            if (teamCount != 2 && teamCount != 4 && teamCount != 8)
            {
                ModelState.AddModelError("SelectedTeamIds", "The number of participating teams must be exactly 2, 4, or 8.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Teams = await _context.Teams.ToListAsync();
                return View(tournament);
            }

            int durationDays = (int)Math.Log2(teamCount);
            if (tournament.StartDate.HasValue)
            {
                tournament.EndDate = tournament.StartDate.Value.AddDays(durationDays);
            }

            tournament.Status = tournament.CurrentStatus;

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            foreach (var teamId in tournament.SelectedTeamIds)
            {
                _context.TournamentTeams.Add(new TournamentTeam
                {
                    TournamentId = tournament.TournamentId,
                    TeamId = teamId
                });
            }

            var rng = new Random();
            var shuffledTeams = tournament.SelectedTeamIds.OrderBy(x => rng.Next()).ToList();

            DateTime currentMatchTime = tournament.StartDate.Value.Date.AddHours(10);

            for (int i = 0; i < shuffledTeams.Count; i += 2)
            {
                _context.Matches.Add(new Match
                {
                    TournamentId = tournament.TournamentId,
                    TeamAId = shuffledTeams[i],
                    TeamBId = shuffledTeams[i + 1],
                    ScoreA = 0,
                    ScoreB = 0,
                    ScheduledAt = currentMatchTime,
                    MatchType = "Auto"
                });

                currentMatchTime = currentMatchTime.AddHours(2);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .Include(t => t.TournamentTeams)
                .Include(t => t.Tickets)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament != null)
            {
                _context.Matches.RemoveRange(tournament.Matches);
                _context.TournamentTeams.RemoveRange(tournament.TournamentTeams);
                _context.Tickets.RemoveRange(tournament.Tickets);

                _context.Tournaments.Remove(tournament);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}