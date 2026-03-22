using System;
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
            var mctContext = _context.Tickets.Include(t => t.Tournament).Include(t => t.User);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        public IActionResult Create()
        {
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");

            var statuses = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Used", Text = "Used" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
            ViewData["Status"] = new SelectList(statuses, "Value", "Text");

            var newTicket = new Ticket { QrCode = GenerateUniqueQrCode() };
            return View(newTicket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,UserId,TournamentId,PurchaseDate,Status,QrCode")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.PurchaseDate.HasValue)
                {
                    ticket.PurchaseDate = DateTime.SpecifyKind(ticket.PurchaseDate.Value, DateTimeKind.Utc);
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            return View(ticket);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);

            var statuses = new System.Collections.Generic.List<SelectListItem> {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Used", Text = "Used" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
            ViewData["Status"] = new SelectList(statuses, "Value", "Text", ticket.Status);

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,UserId,TournamentId,PurchaseDate,Status,QrCode")] Ticket ticket)
        {
            if (id != ticket.TicketId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ticket.PurchaseDate.HasValue)
                    {
                        ticket.PurchaseDate = DateTime.SpecifyKind(ticket.PurchaseDate.Value, DateTimeKind.Utc);
                    }

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
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "Description", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", ticket.UserId);
            return View(ticket);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
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
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }

        private string GenerateUniqueQrCode()
        {
            var random = new Random();
            string newQrCode;
            bool exists;
            do
            {
                char[] digits = new char[16];
                for (int i = 0; i < 16; i++)
                {
                    digits[i] = (char)('0' + random.Next(0, 10));
                }
                newQrCode = new string(digits);
                exists = _context.Tickets.Any(t => t.QrCode == newQrCode);
            } while (exists);

            return newQrCode;
        }
    }
}