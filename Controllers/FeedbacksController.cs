using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Childcare.Areas.Identity.Data;
using Childcare.Data;

namespace Childcare.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ChildcareContext _context;

        public FeedbacksController(ChildcareContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var childcareContext = _context.Feedbacks.Include(f => f.Customer).Include(f => f.Reservation).Include(f => f.Service);
            return View(await childcareContext.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Reservation)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID");
            ViewData["ReservationID"] = new SelectList(_context.Reservations, "ReservationID", "ReservationID");
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID");
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackID,ServiceID,CustomerID,Comment,Rate,ReservationID")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", feedback.CustomerID);
            ViewData["ReservationID"] = new SelectList(_context.Reservations, "ReservationID", "ReservationID", feedback.ReservationID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", feedback.ServiceID);
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", feedback.CustomerID);
            ViewData["ReservationID"] = new SelectList(_context.Reservations, "ReservationID", "ReservationID", feedback.ReservationID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", feedback.ServiceID);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackID,ServiceID,CustomerID,Comment,Rate,ReservationID")] Feedback feedback)
        {
            if (id != feedback.FeedbackID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", feedback.CustomerID);
            ViewData["ReservationID"] = new SelectList(_context.Reservations, "ReservationID", "ReservationID", feedback.ReservationID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", feedback.ServiceID);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Reservation)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackID == id);
        }
    }
}
