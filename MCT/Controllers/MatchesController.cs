using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCT.Models;

namespace MCT.Controllers
{
    public class MatchesController : Controller
    {
        private readonly MctContext _context;

        public MatchesController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var matches = _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB);
            return View(await matches.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.TeamA)
                .Include(m => m.TeamB)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();

            return View(match);
        }
    }
}