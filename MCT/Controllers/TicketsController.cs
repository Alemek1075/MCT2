using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly MctContext _context;

        public TicketsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                int currentUserId = int.Parse(User.FindFirst("UserId").Value);
                query = query.Where(t => t.UserId == currentUserId);
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null) return NotFound();

            if (!User.IsInRole("Admin") && ticket.UserId.ToString() != User.FindFirst("UserId").Value)
            {
                TempData["ErrorMessage"] = "You do not have permission to view this ticket.";
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewBag.TournamentsList = await _context.Tournaments.ToListAsync();

            if (User.IsInRole("Admin"))
            {
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,TournamentId,UserId,PurchaseDate")] Ticket ticket)
        {
            ModelState.Remove("Status");
            ModelState.Remove("Tournament");
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            ModelState.Remove("StatusNavigation");

            if (!User.IsInRole("Admin"))
            {
                ticket.UserId = int.Parse(User.FindFirst("UserId").Value);
            }

            var tournament = await _context.Tournaments.Include(t => t.Tickets).FirstOrDefaultAsync(t => t.TournamentId == ticket.TournamentId);

            if (tournament != null)
            {
                var startDate = tournament.StartDate?.Date ?? DateTime.UtcNow.Date;
                var availableFrom = startDate.AddMonths(-1);

                if (!ticket.PurchaseDate.HasValue)
                {
                    ModelState.AddModelError("PurchaseDate", "Issue date is required.");
                }
                else
                {
                    var pDate = ticket.PurchaseDate.Value.Date;
                    if (pDate < availableFrom || pDate > startDate)
                    {
                        ModelState.AddModelError("PurchaseDate", $"Issue date must be between {availableFrom:MM/dd/yyyy} and {startDate:MM/dd/yyyy}.");
                    }
                }

                int activeTickets = tournament.Tickets.Count(t => t.Status != "Canceled");
                if (tournament.Places <= 0 || activeTickets >= tournament.Places)
                {
                    ModelState.AddModelError("TournamentId", "This event is completely sold out! No seats left.");
                }
            }

            ticket.Status = "Valid";

            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);

            if (User.IsInRole("Admin"))
            {
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            }

            ViewBag.TournamentsList = await _context.Tournaments.ToListAsync();

            return View(ticket);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,TournamentId,UserId,PurchaseDate,Status")] Ticket ticket)
        {
            if (id != ticket.TicketId) return NotFound();

            ModelState.Remove("Tournament");
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            ModelState.Remove("StatusNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);

            return View(ticket);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                if (await _context.Payments.AnyAsync(p => p.TicketId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete: This ticket has registered payments.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}