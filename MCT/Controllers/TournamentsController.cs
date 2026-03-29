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
    public class TournamentsController : Controller
    {
        private readonly MctContext _context;

        public TournamentsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Tournaments.Include(t => t.StatusNavigation);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        public IActionResult Create()
        {
            var statuses = new List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Automatic" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
            ViewData["Status"] = new SelectList(statuses, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TournamentId,Description,Location,StartDate,EndDate,Price,Status")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                var today = DateTime.UtcNow.Date;

                if (tournament.Status == "Active")
                {
                    if (tournament.StartDate.HasValue && tournament.EndDate.HasValue)
                    {
                        var start = tournament.StartDate.Value.Date;
                        var end = tournament.EndDate.Value.Date;

                        if (today < start)
                            tournament.Status = "Planned";
                        else if (today >= start && today <= end)
                            tournament.Status = "Ongoing";
                        else
                            tournament.Status = "Completed";
                    }
                    else
                    {
                        tournament.Status = "Planned";
                    }
                }


                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var statuses = new List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Automatic" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
            ViewData["Status"] = new SelectList(statuses, "Value", "Text", tournament.Status);
            return View(tournament);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            var statuses = new List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Automatic" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };

            var currentStatus = tournament.Status == "Canceled" ? "Canceled" : "Active";
            ViewData["Status"] = new SelectList(statuses, "Value", "Text", currentStatus);

            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TournamentId,Description,Location,StartDate,EndDate,Price,Status")] Tournament tournament)
        {
            if (id != tournament.TournamentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (tournament.StartDate.HasValue)
                        tournament.StartDate = DateTime.SpecifyKind(tournament.StartDate.Value, DateTimeKind.Utc);
                    if (tournament.EndDate.HasValue)
                        tournament.EndDate = DateTime.SpecifyKind(tournament.EndDate.Value, DateTimeKind.Utc);

                    if (tournament.Status == "Active")
                    {
                        var today = DateTime.UtcNow.Date;
                        if (tournament.StartDate.HasValue && tournament.EndDate.HasValue)
                        {
                            var start = tournament.StartDate.Value.Date;
                            var end = tournament.EndDate.Value.Date;

                            if (today < start) tournament.Status = "Planned";
                            else if (today >= start && today <= end) tournament.Status = "Ongoing";
                            else tournament.Status = "Completed";
                        }
                        else
                        {
                            tournament.Status = "Planned";
                        }
                    }

                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.TournamentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var statuses = new List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Automatic" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
            ViewData["Status"] = new SelectList(statuses, "Value", "Text", tournament.Status);
            return View(tournament);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tournament = await _context.Tournaments
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament == null) return NotFound();

            return View(tournament);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .Include(t => t.Tickets)
                .Include(t => t.TournamentTeams)
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TournamentId == id);

            if (tournament != null)
            {
                List<string> dependencies = new List<string>();
                if (tournament.Matches.Any()) dependencies.Add("Matches");
                if (tournament.Tickets.Any()) dependencies.Add("Tickets");
                if (tournament.TournamentTeams.Any()) dependencies.Add("TournamentTeams");

                if (dependencies.Any())
                {
                    ViewBag.ErrorMessage = $"Cannot delete because this object is used in: {string.Join(", ", dependencies)}";
                    return View(tournament);
                }

                _context.Tournaments.Remove(tournament);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.TournamentId == id);
        }
    }
}