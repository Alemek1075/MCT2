using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .Include(t => t.Matches).ThenInclude(m => m.TeamA)
                .Include(t => t.Matches).ThenInclude(m => m.TeamB)
                .Include(t => t.TournamentTeams).ThenInclude(tt => tt.Team)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Teams = await _context.Teams.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Tournament tournament)
        {
            ModelState.Remove("EndDate");
            ModelState.Remove("Status");

            int teamCount = tournament.SelectedTeamIds?.Count ?? 0;

            if (teamCount != 2 && teamCount != 4 && teamCount != 8)
            {
                ModelState.AddModelError("SelectedTeamIds", "The number of participating teams must be exactly 2, 4, or 8.");
            }

            int durationDays = teamCount > 0 ? (int)Math.Log2(teamCount) : 0;
            DateTime calculatedEndDate = tournament.StartDate.Value.AddDays(durationDays);

            bool isOverlap = await _context.Tournaments.AnyAsync(t =>
                t.StartDate.HasValue && t.EndDate.HasValue &&
                tournament.StartDate.Value.Date <= t.EndDate.Value.Date &&
                calculatedEndDate.Date >= t.StartDate.Value.Date);

            if (isOverlap)
            {
                ModelState.AddModelError("StartDate", "Dates overlap with an existing tournament! Choose another date.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Teams = await _context.Teams.ToListAsync();
                return View(tournament);
            }

            tournament.EndDate = calculatedEndDate;
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

            int totalRounds = (int)Math.Log2(teamCount);
            int currentRoundMatchesCount = teamCount / 2;
            DateTime roundDate = tournament.StartDate.Value.Date;

            for (int r = 1; r <= totalRounds; r++)
            {
                DateTime currentMatchTime = roundDate.AddHours(10);

                for (int m = 0; m < currentRoundMatchesCount; m++)
                {
                    var newMatch = new Match
                    {
                        TournamentId = tournament.TournamentId,
                        ScoreA = 0,
                        ScoreB = 0,
                        ScheduledAt = currentMatchTime,
                        MatchType = "Auto"
                    };

                    if (r == 1)
                    {
                        newMatch.TeamAId = shuffledTeams[m * 2];
                        newMatch.TeamBId = shuffledTeams[m * 2 + 1];
                    }

                    _context.Matches.Add(newMatch);
                    currentMatchTime = currentMatchTime.AddHours(2);
                }

                currentRoundMatchesCount /= 2;
                roundDate = roundDate.AddDays(1);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            ViewBag.Status = new SelectList(await _context.TournamentStatuses.ToListAsync(), "StatusName", "StatusName", tournament.Status);
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Tournament tournament)
        {
            if (id != tournament.TournamentId) return NotFound();

            ModelState.Remove("EndDate");

            var existingTournament = await _context.Tournaments
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.TournamentId == id);

            if (existingTournament == null) return NotFound();

            if (existingTournament.StartDate?.Date != tournament.StartDate?.Date)
            {
                bool hasScores = existingTournament.Matches.Any(m => m.ScoreA > 0 || m.ScoreB > 0);

                if (hasScores)
                {
                    ModelState.AddModelError("StartDate", "Cannot change Start Date because some matches already have scores recorded.");
                }
                else
                {
                    var timeShift = tournament.StartDate.Value - existingTournament.StartDate.Value;
                    DateTime newEndDate = existingTournament.EndDate.Value.Add(timeShift);

                    bool isOverlap = await _context.Tournaments.AnyAsync(t =>
                        t.TournamentId != existingTournament.TournamentId &&
                        t.StartDate.HasValue && t.EndDate.HasValue &&
                        tournament.StartDate.Value.Date <= t.EndDate.Value.Date &&
                        newEndDate.Date >= t.StartDate.Value.Date);

                    if (isOverlap)
                    {
                        ModelState.AddModelError("StartDate", "New dates overlap with another existing tournament.");
                    }
                    else
                    {
                        foreach (var match in existingTournament.Matches)
                        {
                            if (match.ScheduledAt.HasValue)
                            {
                                match.ScheduledAt = match.ScheduledAt.Value.Add(timeShift);
                            }
                        }
                        existingTournament.EndDate = newEndDate;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Status = new SelectList(await _context.TournamentStatuses.ToListAsync(), "StatusName", "StatusName", tournament.Status);
                return View(tournament);
            }

            existingTournament.Description = tournament.Description;
            existingTournament.Location = tournament.Location;
            existingTournament.Price = tournament.Price;
            existingTournament.Status = tournament.Status;
            existingTournament.Places = tournament.Places;
            if (tournament.StartDate.HasValue)
            {
                existingTournament.StartDate = tournament.StartDate;
            }

            try
            {
                _context.Update(existingTournament);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw; }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments.FirstOrDefaultAsync(m => m.TournamentId == id);
            if (tournament == null) return NotFound();

            return View(tournament);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .Include(t => t.TournamentTeams)
                .Include(t => t.Tickets)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament != null)
            {
                var matchIds = tournament.Matches.Select(m => m.MatchId).ToList();
                if (await _context.Stats.AnyAsync(s => matchIds.Contains(s.MatchId ?? 0)))
                {
                    TempData["ErrorMessage"] = "Cannot delete: Event has matches with recorded player stats.";
                    return RedirectToAction(nameof(Index));
                }

                var ticketIds = tournament.Tickets.Select(t => t.TicketId).ToList();
                if (await _context.Payments.AnyAsync(p => ticketIds.Contains(p.TicketId ?? 0)))
                {
                    TempData["ErrorMessage"] = "Cannot delete: Tickets for this event have registered payments.";
                    return RedirectToAction(nameof(Index));
                }

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