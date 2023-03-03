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
    public class RegistrationController : Controller
    {
        private readonly MVCSMS _context;

        public RegistrationController(MVCSMS context)
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

        // GET: Registration
        public async Task<IActionResult> AdminIndex()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var mVCSMS = _context.Registration.Include(r => r.attendee).Include(r => r.seminar);
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(await mVCSMS.ToListAsync());
        }

        // GET: Registration/Details/5
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

            var registration = await _context.Registration
                .Include(r => r.attendee)
                .Include(r => r.seminar)
                .FirstOrDefaultAsync(m => m.id == id);
            if (registration == null)
            {
                return NotFound();
            }
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(registration);
        }

        // GET: Registration/Create
        public IActionResult AdminCreate()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            ViewData["attendeeId"] = new SelectList(_context.Person, "id", "email");
            ViewData["seminarId"] = new SelectList(_context.Seminar, "id", "topic");
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View();
        }

        // POST: Registration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("id,attendeeId,seminarId")] Registration registration)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                // check if the attendee is already registered for the seminar
                var isRegistered = _context.Registration.Where(r => r.attendeeId == registration.attendeeId && r.seminarId == registration.seminarId).FirstOrDefault();
                if (isRegistered != null)
                {
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "Attendee is already registered for this seminar";
                    return RedirectToAction("AdminCreate");
                }
                _context.Add(registration);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Registration Created Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            ViewData["attendeeId"] = new SelectList(_context.Person, "id", "email", registration.attendeeId);
            ViewData["seminarId"] = new SelectList(_context.Seminar, "id", "topic", registration.seminarId);
            return View(registration);
        }

        // GET: Registration/Edit/5
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

            var registration = await _context.Registration.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }
            ViewData["attendeeId"] = new SelectList(_context.Person, "id", "email", registration.attendeeId);
            ViewData["seminarId"] = new SelectList(_context.Seminar, "id", "topic", registration.seminarId);
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(registration);
        }

        // POST: Registration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("id,attendeeId,seminarId")] Registration registration)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id != registration.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var isRegistered = _context.Registration.Where(r => r.attendeeId == registration.attendeeId && r.seminarId == registration.seminarId).FirstOrDefault();
                if (isRegistered != null)
                {
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "Attendee is already registered for this seminar";
                    return RedirectToAction("AdminCreate");
                }
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Registration Edited Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            ViewData["attendeeId"] = new SelectList(_context.Person, "id", "email", registration.attendeeId);
            ViewData["seminarId"] = new SelectList(_context.Seminar, "id", "topic", registration.seminarId);
            return View(registration);
        }

        // GET: Registration/Delete/5
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

            var registration = await _context.Registration
                .Include(r => r.attendee)
                .Include(r => r.seminar)
                .FirstOrDefaultAsync(m => m.id == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: Registration/Delete/5
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
            var registration = await _context.Registration.FindAsync(id);
            _context.Registration.Remove(registration);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Registration Deleted Successful";
            return RedirectToAction(nameof(AdminIndex));
        }

        private bool RegistrationExists(int id)
        {
            return _context.Registration.Any(e => e.id == id);
        }
    }
}
