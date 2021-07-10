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
    public class ServicesController : Controller
    {
        private readonly ChildcareContext _context;

        public ServicesController(ChildcareContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var childcareContext = _context.Services.Include(s => s.Specialty).Include(s => s.Staff).Include(s => s.Status);
            return View(await childcareContext.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Specialty)
                .Include(s => s.Staff)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(m => m.ServiceID == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID");
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID");
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceID,ServiceName,SpecialtyID,Thumbnail,Description,Price,StatusID,CreatedDate,UpdatedDate,StaffID")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", service.SpecialtyID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", service.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", service.StatusID);
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", service.SpecialtyID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", service.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", service.StatusID);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceID,ServiceName,SpecialtyID,Thumbnail,Description,Price,StatusID,CreatedDate,UpdatedDate,StaffID")] Service service)
        {
            if (id != service.ServiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceID))
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
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyID", service.SpecialtyID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", service.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", service.StatusID);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Specialty)
                .Include(s => s.Staff)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(m => m.ServiceID == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceID == id);
        }
    }
}
