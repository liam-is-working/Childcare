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
    public class StaffsController : Controller
    {
        private readonly ChildcareContext _context;

        public StaffsController(ChildcareContext context)
        {
            _context = context;
        }

        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            var childcareContext = _context.Staffs.Include(s => s.ChildCareUser).Include(s => s.Specialty);
            return View(await childcareContext.ToListAsync());
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.ChildCareUser)
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            ViewData["ChildcareUserId"] = new SelectList(_context.Set<ChildCareUser>(), "Id", "Id");
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID");
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChildcareUserId,StaffID,SpecialtyID")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChildcareUserId"] = new SelectList(_context.Set<ChildCareUser>(), "Id", "Id", staff.ChildcareUserId);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", staff.SpecialtyID);
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewData["ChildcareUserId"] = new SelectList(_context.Set<ChildCareUser>(), "Id", "Id", staff.ChildcareUserId);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", staff.SpecialtyID);
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChildcareUserId,StaffID,SpecialtyID")] Staff staff)
        {
            if (id != staff.StaffID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffID))
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
            ViewData["ChildcareUserId"] = new SelectList(_context.Set<ChildCareUser>(), "Id", "Id", staff.ChildcareUserId);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", staff.SpecialtyID);
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.ChildCareUser)
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(e => e.StaffID == id);
        }
    }
}
