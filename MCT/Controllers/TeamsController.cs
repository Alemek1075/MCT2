using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class TeamsController : Controller
    {
        private readonly MctContext _context;
        public TeamsController(MctContext context) { _context = context; }

        public async Task<IActionResult> Index() { return View(await _context.Teams.Include(t => t.Players).ToListAsync()); }

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); return View(await _context.Teams.Include(t => t.Players).ThenInclude(p => p.User).FirstOrDefaultAsync(m => m.TeamId == id)); }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TeamId,Name,ShortCode,Region")] Team team)
        {
            if (ModelState.IsValid) { _context.Add(team); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(team);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,Name,ShortCode,Region")] Team team)
        {
            if (id != team.TeamId) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(team); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                if (await _context.TournamentTeams.AnyAsync(tt => tt.TeamId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete: Team is participating in a tournament."; return RedirectToAction(nameof(Index));
                }
                if (await _context.Matches.AnyAsync(m => m.TeamAId == id || m.TeamBId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete: Team has scheduled or played matches."; return RedirectToAction(nameof(Index));
                }
                if (await _context.Players.AnyAsync(p => p.TeamId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete: Team has players assigned. Reassign them first."; return RedirectToAction(nameof(Index));
                }
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}