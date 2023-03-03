using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly MVCSMS _context;

        public AdminController(MVCSMS context)
        {
            _context = context;
        }

        public bool isAdminLogin()
        {
            var adminId = HttpContext.Session.GetInt32("adminId");
            if (adminId != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,username,password")] Admin admin)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Organizer Created Successful";
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,username,password")] Admin admin)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id != admin.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Organizer Created Successful";
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var admin = await _context.Admin.FindAsync(id);
            _context.Admin.Remove(admin);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Organizer Created Successful";
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.id == id);
        }
    }
}
