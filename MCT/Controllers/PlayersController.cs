using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class PlayersController : Controller
    {
        private readonly MctContext _context;

        public PlayersController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Players.Include(p => p.Team).Include(p => p.User);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players
                .Include(p => p.Team).Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null) return NotFound();
            return View(player);
        }

        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerId,UserId,TeamId")] Player player)
        {
            if (_context.Players.Any(p => p.UserId == player.UserId))
            {
                ModelState.AddModelError("UserId", "Цей користувач вже доданий як гравець в одну з команд!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", player.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", player.UserId);
            return View(player);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", player.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", player.UserId);
            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlayerId,UserId,TeamId")] Player player)
        {
            if (id != player.PlayerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { throw; }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", player.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Username", player.UserId);
            return View(player);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players
                .Include(p => p.Team).Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null) return NotFound();
            return View(player);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}