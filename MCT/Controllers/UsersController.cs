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
    public class UsersController : Controller
    {
        private readonly MctContext _context;

        public UsersController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mctContext = _context.Users.Include(u => u.RoleNavigation);
            return View(await mctContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(_context.UserRoles, "RoleName", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,PasswordHash,Role")] User user)
        {
            // Перевірка на унікальність імені
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Користувач з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(_context.UserRoles, "RoleName", "RoleName", user.Role);
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["Role"] = new SelectList(_context.UserRoles, "RoleName", "RoleName", user.Role);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,PasswordHash,Role")] User user)
        {
            if (id != user.UserId) return NotFound();

            // Перевірка на унікальність імені для інших юзерів (окрім поточного)
            if (_context.Users.Any(u => u.Username == user.Username && u.UserId != user.UserId))
            {
                ModelState.AddModelError("Username", "Користувач з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Role"] = new SelectList(_context.UserRoles, "RoleName", "RoleName", user.Role);
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}