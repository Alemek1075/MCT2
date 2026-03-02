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
    public class PaymentStatusController : Controller
    {
        private readonly MctContext _context;

        public PaymentStatusController(MctContext context)
        {
            _context = context;
        }

        // GET: PaymentStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentStatuses.ToListAsync());
        }

        // GET: PaymentStatus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentStatus = await _context.PaymentStatuses
                .FirstOrDefaultAsync(m => m.StatusName == id);
            if (paymentStatus == null)
            {
                return NotFound();
            }

            return View(paymentStatus);
        }

        // GET: PaymentStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] PaymentStatus paymentStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentStatus);
        }

        // GET: PaymentStatus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentStatus = await _context.PaymentStatuses.FindAsync(id);
            if (paymentStatus == null)
            {
                return NotFound();
            }
            return View(paymentStatus);
        }

        // POST: PaymentStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StatusName")] PaymentStatus paymentStatus)
        {
            if (id != paymentStatus.StatusName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentStatusExists(paymentStatus.StatusName))
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
            return View(paymentStatus);
        }

        // GET: PaymentStatus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentStatus = await _context.PaymentStatuses
                .FirstOrDefaultAsync(m => m.StatusName == id);
            if (paymentStatus == null)
            {
                return NotFound();
            }

            return View(paymentStatus);
        }

        // POST: PaymentStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var paymentStatus = await _context.PaymentStatuses.FindAsync(id);
            if (paymentStatus != null)
            {
                _context.PaymentStatuses.Remove(paymentStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentStatusExists(string id)
        {
            return _context.PaymentStatuses.Any(e => e.StatusName == id);
        }
    }
}
