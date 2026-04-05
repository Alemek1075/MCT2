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
    public class TicketsController : Controller
    {
        private readonly MctContext _context;

        public TicketsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Tickets.Include(t => t.StatusNavigation).Include(t => t.Tournament).Include(t => t.User);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets
                .Include(t => t.StatusNavigation).Include(t => t.Tournament).Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            ViewBag.TournamentsList = await _context.Tournaments.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,TournamentId,UserId")] Ticket ticket)
        {
            ModelState.Remove("Status");
            ModelState.Remove("PurchaseDate");
            ModelState.Remove("Tournament");
            ModelState.Remove("User");
            ModelState.Remove("Payments");
            ModelState.Remove("StatusNavigation");

            var tournament = await _context.Tournaments.Include(t => t.Tickets).FirstOrDefaultAsync(t => t.TournamentId == ticket.TournamentId);

            if (tournament != null)
            {
                var today = DateTime.UtcNow.Date;
                var startDate = tournament.StartDate?.Date ?? today;
                var endDate = tournament.EndDate?.Date ?? today;
                var availableFrom = startDate.AddMonths(-1);

                if (today < availableFrom || today > endDate)
                {
                    ModelState.AddModelError("TournamentId", $"Tickets are only available between {availableFrom:MM/dd/yyyy} and {endDate:MM/dd/yyyy}.");
                }

                int activeTickets = tournament.Tickets.Count(t => t.Status != "Canceled");

                if (tournament.Places <= 0 || activeTickets >= tournament.Places)
                {
                    ModelState.AddModelError("TournamentId", "This event is completely sold out! No seats left.");
                }
            }

            ticket.PurchaseDate = DateTime.UtcNow.Date;
            ticket.Status = "Valid";

            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            ViewBag.TournamentsList = await _context.Tournaments.ToListAsync();
            return View(ticket);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets.Include(t => t.Tournament).FirstOrDefaultAsync(t => t.TicketId == id);
            if (ticket == null) return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int UserId, bool IsCanceled)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Tournament)
                .ThenInclude(tour => tour.Tickets)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null) return NotFound();

            string newStatus = IsCanceled ? "Canceled" : "Valid";

            if (ticket.Status == "Canceled" && newStatus == "Valid")
            {
                int activeTickets = ticket.Tournament.Tickets.Count(t => t.Status != "Canceled");
                if (activeTickets >= ticket.Tournament.Places)
                {
                    ModelState.AddModelError("", "Cannot restore ticket: The event is completely sold out!");
                    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", UserId);
                    return View(ticket);
                }
            }

            ticket.UserId = UserId;
            ticket.Status = newStatus;

            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw; }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets.Include(t => t.Tournament).Include(t => t.User).FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}