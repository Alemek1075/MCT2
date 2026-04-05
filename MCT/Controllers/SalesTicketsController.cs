using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class SalesTicketsController : Controller
    {
        private readonly MctContext _context;

        public SalesTicketsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.Tickets)
                .ToListAsync();

            return View(tournaments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Tickets)
                .FirstOrDefaultAsync(t => t.TournamentId == id);

            if (tournament == null) return NotFound();

            var startDate = tournament.StartDate?.Date ?? DateTime.UtcNow.Date;
            var fromDate = startDate.AddMonths(-1);

            var salesData = new Dictionary<DateTime, int>();

            for (var d = fromDate; d <= startDate; d = d.AddDays(1))
            {
                salesData[d] = tournament.Tickets.Count(t => t.Status != "Canceled" && t.PurchaseDate.HasValue && t.PurchaseDate.Value.Date == d.Date);
            }

            ViewBag.Tournament = tournament;
            return View(salesData);
        }
    }
}