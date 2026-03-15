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

            if (_context.Users.Any(u => u.Username == user.Username && u.UserId != user.UserId))
            {
                ModelState.AddModelError("Username", "Користувач з таким ім'ям вже існує.");
            }

            // Перевірка: не даємо змінити роль, якщо юзер вже є в Players
            if (user.Role != "Player" && _context.Players.Any(p => p.UserId == user.UserId))
            {
                ModelState.AddModelError("Role", "Цей користувач зареєстрований як гравець команди. Змінити роль неможливо.");
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
            var user = await _context.Users
                .Include(u => u.Players)
                .Include(u => u.Tickets)
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user != null)
            {
                List<string> dependencies = new List<string>();
                if (user.Players.Any()) dependencies.Add("Гравцях (Players)");
                if (user.Tickets.Any()) dependencies.Add("Квитках (Tickets)");

                if (dependencies.Any())
                {
                    ViewBag.ErrorMessage = $"Не можна видалити, бо цей об'єкт задіяний в: {string.Join(", ", dependencies)}";
                    return View(user);
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}