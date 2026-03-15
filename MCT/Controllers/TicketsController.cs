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
            var mctContext = _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User);
            return View(await mctContext.ToListAsync());
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

            return View(ticket);
        }

        public IActionResult Create()
        {
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName");
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        private string GenerateUniqueQrCode()
        {
            var random = new Random();
            string newQrCode;
            bool exists;
            do
            {
                char[] digits = new char[15];
                for (int i = 0; i < 15; i++) digits[i] = (char)('0' + random.Next(0, 10));
                newQrCode = new string(digits);
                exists = _context.Tickets.Any(t => t.QrCode == newQrCode);
            } while (exists);

            return newQrCode;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,UserId,TournamentId,PurchaseDate,Status")] Ticket ticket)
        {
            if (ticket.PurchaseDate.HasValue)
            {
                ticket.PurchaseDate = DateTime.SpecifyKind(ticket.PurchaseDate.Value, DateTimeKind.Utc);
            }

            if (ticket.TournamentId.HasValue && ticket.PurchaseDate.HasValue)
            {
                var tournament = await _context.Tournaments.AsNoTracking().FirstOrDefaultAsync(t => t.TournamentId == ticket.TournamentId);
                if (tournament != null && tournament.EndDate.HasValue && ticket.PurchaseDate.Value.Date > tournament.EndDate.Value.Date)
                {
                    ModelState.AddModelError("PurchaseDate", "Purchase date cannot be later than the event's end date.");
                }
            }

            if (ModelState.IsValid)
            {
                ticket.QrCode = GenerateUniqueQrCode();
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            return View(ticket);
        }

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
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,UserId,TournamentId,PurchaseDate,Status,QrCode")] Ticket ticket)
        {
            if (id != ticket.TicketId) return NotFound();

            if (ticket.PurchaseDate.HasValue)
            {
                ticket.PurchaseDate = DateTime.SpecifyKind(ticket.PurchaseDate.Value, DateTimeKind.Utc);
            }

            if (_context.Tickets.Any(t => t.QrCode == ticket.QrCode && t.TicketId != ticket.TicketId))
            {
                ModelState.AddModelError("QrCode", "This QR Code is already in use.");
            }

            if (ticket.TournamentId.HasValue && ticket.PurchaseDate.HasValue)
            {
                var tournament = await _context.Tournaments.AsNoTracking().FirstOrDefaultAsync(t => t.TournamentId == ticket.TournamentId);
                if (tournament != null && tournament.EndDate.HasValue && ticket.PurchaseDate.Value.Date > tournament.EndDate.Value.Date)
                {
                    ModelState.AddModelError("PurchaseDate", "Purchase date cannot be later than the event's end date.");
                }
            }

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null) _context.Tickets.Remove(ticket);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}