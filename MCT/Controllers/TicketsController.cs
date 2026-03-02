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

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Tickets.Include(t => t.StatusNavigation).Include(t => t.Tournament).Include(t => t.User);
            return View(await mctContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName");
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,UserId,TournamentId,PurchaseDate,Status,QrCode")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,UserId,TournamentId,PurchaseDate,Status,QrCode")] Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
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
                    if (!TicketExists(ticket.TicketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Status"] = new SelectList(_context.TicketStatuses, "StatusName", "StatusName", ticket.Status);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", ticket.TournamentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.StatusNavigation)
                .Include(t => t.Tournament)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
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
    }
}
