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
    public class SeminarController : Controller
    {
        private readonly MVCSMS _context;

        public SeminarController(MVCSMS context)
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

        // GET: Seminar
        public async Task<IActionResult> AdminIndex()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var mVCSMS = _context.Seminar.Include(s => s.Organizer);
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(await mVCSMS.ToListAsync());
        }

        // GET: Seminar/Details/5
        public async Task<IActionResult> AdminDetails(int? id)
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

            var seminar = await _context.Seminar
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.id == id);
            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        // GET: Seminar/Create
        public IActionResult AdminCreate()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            ViewData["OrganizerId"] = new SelectList(_context.Organizer.Where(o=>o.isVerified), "id", "email");
            return View();
        }

        // POST: Seminar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("id,topic,platform,link,linkId,password,type,Seminar_Date,Starting_Time,Ending_Time,OrganizerId")] Seminar seminar)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(seminar);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Seminar Created Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            ViewData["OrganizerId"] = new SelectList(_context.Organizer.Where(o=>o.isVerified), "id", "email", seminar.OrganizerId);
            return View(seminar);
        }

        // GET: Seminar/Edit/5
        public async Task<IActionResult> AdminEdit(int? id)
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

            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }
            ViewData["OrganizerId"] = new SelectList(_context.Organizer.Where(o=>o.isVerified), "id", "email", seminar.OrganizerId);
            return View(seminar);
        }

        // POST: Seminar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("id,topic,platform,link,linkId,password,type,Seminar_Date,Starting_Time,Ending_Time,OrganizerId")] Seminar seminar)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id != seminar.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seminar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeminarExists(seminar.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Seminar Edited Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            ViewData["OrganizerId"] = new SelectList(_context.Organizer.Where(o=>o.isVerified), "id", "email", seminar.OrganizerId);
            return View(seminar);
        }

        // GET: Seminar/Delete/5
        public async Task<IActionResult> AdminDelete(int? id)
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

            var seminar = await _context.Seminar
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(m => m.id == id);
            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        // POST: Seminar/Delete/5
        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var seminar = await _context.Seminar.FindAsync(id);
            _context.Seminar.Remove(seminar);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Seminar Deleted Successful";
            return RedirectToAction(nameof(AdminIndex));
        }

        private bool SeminarExists(int id)
        {
            return _context.Seminar.Any(e => e.id == id);
        }

    }
}
