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
    public class TournamentTeamsController : Controller
    {
        private readonly MctContext _context;

        public TournamentTeamsController(MctContext context)
        {
            _context = context;
        }

        // GET: TournamentTeams
        public async Task<IActionResult> Index()
        {
            var mctContext = _context.TournamentTeams.Include(t => t.Team).Include(t => t.Tournament);
            return View(await mctContext.ToListAsync());
        }

        // GET: TournamentTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentTeam = await _context.TournamentTeams
                .Include(t => t.Team)
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentTeam == null)
            {
                return NotFound();
            }

            return View(tournamentTeam);
        }

        // GET: TournamentTeams/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId");
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId");
            return View();
        }

        // POST: TournamentTeams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TournamentId,TeamId,Placement")] TournamentTeam tournamentTeam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournamentTeam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", tournamentTeam.TeamId);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", tournamentTeam.TournamentId);
            return View(tournamentTeam);
        }

        // GET: TournamentTeams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentTeam = await _context.TournamentTeams.FindAsync(id);
            if (tournamentTeam == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", tournamentTeam.TeamId);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", tournamentTeam.TournamentId);
            return View(tournamentTeam);
        }

        // POST: TournamentTeams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TournamentId,TeamId,Placement")] TournamentTeam tournamentTeam)
        {
            if (id != tournamentTeam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournamentTeam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentTeamExists(tournamentTeam.Id))
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
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", tournamentTeam.TeamId);
            ViewData["TournamentId"] = new SelectList(_context.Tournaments, "TournamentId", "TournamentId", tournamentTeam.TournamentId);
            return View(tournamentTeam);
        }

        // GET: TournamentTeams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentTeam = await _context.TournamentTeams
                .Include(t => t.Team)
                .Include(t => t.Tournament)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentTeam == null)
            {
                return NotFound();
            }

            return View(tournamentTeam);
        }

        // POST: TournamentTeams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournamentTeam = await _context.TournamentTeams.FindAsync(id);
            if (tournamentTeam != null)
            {
                _context.TournamentTeams.Remove(tournamentTeam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentTeamExists(int id)
        {
            return _context.TournamentTeams.Any(e => e.Id == id);
        }
    }
}
