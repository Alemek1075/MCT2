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
    public class MatchTypesController : Controller
    {
        private readonly MctContext _context;

        public MatchTypesController(MctContext context)
        {
            _context = context;
        }

        // GET: MatchTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MatchTypes.ToListAsync());
        }

        // GET: MatchTypes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchType = await _context.MatchTypes
                .FirstOrDefaultAsync(m => m.TypeName == id);
            if (matchType == null)
            {
                return NotFound();
            }

            return View(matchType);
        }

        // GET: MatchTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MatchTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeName")] Models.MatchType matchType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matchType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(matchType);
        }

        // GET: MatchTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchType = await _context.MatchTypes.FindAsync(id);
            if (matchType == null)
            {
                return NotFound();
            }
            return View(matchType);
        }

        // POST: MatchTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TypeName")] Models.MatchType matchType)
        {
            if (id != matchType.TypeName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matchType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchTypeExists(matchType.TypeName))
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
            return View(matchType);
        }

        // GET: MatchTypes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matchType = await _context.MatchTypes
                .FirstOrDefaultAsync(m => m.TypeName == id);
            if (matchType == null)
            {
                return NotFound();
            }

            return View(matchType);
        }

        // POST: MatchTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var matchType = await _context.MatchTypes.FindAsync(id);
            if (matchType != null)
            {
                _context.MatchTypes.Remove(matchType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchTypeExists(string id)
        {
            return _context.MatchTypes.Any(e => e.TypeName == id);
        }
    }
}
