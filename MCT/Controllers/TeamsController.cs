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
    public class TeamsController : Controller
    {
        private readonly MctContext _context;

        public TeamsController(MctContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.Players)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.TeamId == id);

            if (team == null) return NotFound();

            return View(team);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamId,Name,ShortCode,Region")] Team team)
        {
            if (ModelState.IsValid)
            {
                team.MemberCount = 0;
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,Name,ShortCode,Region")] Team team)
        {
            if (id != team.TeamId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTeam = await _context.Teams
                        .Include(t => t.Players)
                        .FirstOrDefaultAsync(t => t.TeamId == id);

                    if (existingTeam == null) return NotFound();

                    existingTeam.Name = team.Name;
                    existingTeam.ShortCode = team.ShortCode;
                    existingTeam.Region = team.Region;
                    existingTeam.MemberCount = existingTeam.Players.Count;

                    _context.Update(existingTeam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.TeamId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.TeamId == id);
            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams
                .Include(t => t.MatchTeamAs)
                .Include(t => t.MatchTeamBs)
                .Include(t => t.MatchWinners)
                .Include(t => t.Players)
                .Include(t => t.TournamentTeams)
                .FirstOrDefaultAsync(m => m.TeamId == id);

            if (team != null)
            {
                List<string> dependencies = new List<string>();
                if (team.MatchTeamAs.Any() || team.MatchTeamBs.Any() || team.MatchWinners.Any()) dependencies.Add("Matches");
                if (team.Players.Any()) dependencies.Add("Players");
                if (team.TournamentTeams.Any()) dependencies.Add("TournamentTeams");

                if (dependencies.Any())
                {
                    ViewBag.ErrorMessage = $"Cannot delete because this object is used in: {string.Join(", ", dependencies)}";
                    return View(team);
                }

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }
    }
}