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
    public class BlogsController : Controller
    {
        private readonly ChildcareContext _context;

        public BlogsController(ChildcareContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var childcareContext = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.Staff).Include(b => b.Status);
            return View(await childcareContext.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.Staff)
                .Include(b => b.Status)
                .FirstOrDefaultAsync(m => m.BlogID == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.BlogCategories, "CategoryID", "CategoryID");
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID");
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogID,Thumbnail,Tile,StaffID,Description,CreadtedDate,UpdatedDate,StatusID,CategoryID")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.BlogCategories, "CategoryID", "CategoryID", blog.CategoryID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", blog.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", blog.StatusID);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.BlogCategories, "CategoryID", "CategoryID", blog.CategoryID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", blog.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", blog.StatusID);
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogID,Thumbnail,Tile,StaffID,Description,CreadtedDate,UpdatedDate,StatusID,CategoryID")] Blog blog)
        {
            if (id != blog.BlogID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogID))
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
            ViewData["CategoryID"] = new SelectList(_context.BlogCategories, "CategoryID", "CategoryID", blog.CategoryID);
            ViewData["StaffID"] = new SelectList(_context.Staffs, "StaffID", "StaffID", blog.StaffID);
            ViewData["StatusID"] = new SelectList(_context.Statuses, "StatusID", "StatusID", blog.StatusID);
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.Staff)
                .Include(b => b.Status)
                .FirstOrDefaultAsync(m => m.BlogID == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogID == id);
        }
    }
}
