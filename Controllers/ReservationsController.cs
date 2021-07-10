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
    public class ReservationsController : Controller
    {
        private readonly ChildcareContext _context;

        public ReservationsController(ChildcareContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var childcareContext = _context.Reservations.Include(r => r.Customer).Include(r => r.Patient).Include(r => r.Service).Include(r => r.Staff);
            return View(await childcareContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Patient)
                .Include(r => r.Service)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID");
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID");
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID");
            ViewData["StaffAssignedID"] = new SelectList(_context.Staffs, "StaffID", "StaffID");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationID,CustomerID,PatientID,ServiceID,StaffAssignedID,OpenTime,CheckInTime,CreatedDate,UpdatedDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", reservation.CustomerID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", reservation.PatientID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", reservation.ServiceID);
            ViewData["StaffAssignedID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", reservation.StaffAssignedID);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", reservation.CustomerID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", reservation.PatientID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", reservation.ServiceID);
            ViewData["StaffAssignedID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", reservation.StaffAssignedID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationID,CustomerID,PatientID,ServiceID,StaffAssignedID,OpenTime,CheckInTime,CreatedDate,UpdatedDate")] Reservation reservation)
        {
            if (id != reservation.ReservationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", reservation.CustomerID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "PatientID", reservation.PatientID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", reservation.ServiceID);
            ViewData["StaffAssignedID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", reservation.StaffAssignedID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Patient)
                .Include(r => r.Service)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationID == id);
        }
    }
}
