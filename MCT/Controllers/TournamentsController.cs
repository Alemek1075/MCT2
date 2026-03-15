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
            if (tournament.StartDate.HasValue) tournament.StartDate = DateTime.SpecifyKind(tournament.StartDate.Value, DateTimeKind.Utc);
            if (tournament.EndDate.HasValue) tournament.EndDate = DateTime.SpecifyKind(tournament.EndDate.Value, DateTimeKind.Utc);

            if (ModelState.IsValid)
            {
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
            ViewData["Status"] = new SelectList(statuses, "Value", "Text", tournament.Status);
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TournamentId,Description,Location,StartDate,EndDate,Price,Status")] Tournament tournament)
        {
            if (id != tournament.TournamentId) return NotFound();

            if (tournament.StartDate.HasValue) tournament.StartDate = DateTime.SpecifyKind(tournament.StartDate.Value, DateTimeKind.Utc);
            if (tournament.EndDate.HasValue) tournament.EndDate = DateTime.SpecifyKind(tournament.EndDate.Value, DateTimeKind.Utc);

            if (tournament.StartDate.HasValue && tournament.EndDate.HasValue)
            {
                bool invalidMatches = await _context.Matches
                    .Where(m => m.TournamentId == tournament.TournamentId && m.ScheduledAt.HasValue)
                    .AnyAsync(m => m.ScheduledAt.Value.Date < tournament.StartDate.Value.Date || m.ScheduledAt.Value.Date > tournament.EndDate.Value.Date);

                if (invalidMatches) ModelState.AddModelError("StartDate", "Existing matches fall outside the new date range.");

                bool invalidTickets = await _context.Tickets
                    .Where(t => t.TournamentId == tournament.TournamentId && t.PurchaseDate.HasValue)
                    .AnyAsync(t => t.PurchaseDate.Value.Date > tournament.EndDate.Value.Date);

                if (invalidTickets) ModelState.AddModelError("EndDate", "Existing tickets have purchase dates after the new end date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
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