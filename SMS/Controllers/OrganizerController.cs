using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using SMS.Models;
using System.Text;

namespace SMS.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly MVCSMS _context;

        public OrganizerController(MVCSMS context)
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

        // GET: Organizer
        public async Task<IActionResult> AdminIndex()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(await _context.Organizer.ToListAsync());
        }

        // GET: Organizer/Details/5
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

            var organizer = await _context.Organizer
                .FirstOrDefaultAsync(m => m.id == id);
            if (organizer == null)
            {
                return NotFound();
            }

            return View(organizer);
        }

        // GET: Organizer/Create
        public IActionResult AdminCreate()
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            return View();
        }

        // POST: Organizer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("id,email,password,name,isVerified")] Organizer organizer)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(organizer);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Organizer Created Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(organizer);
        }

        // GET: Organizer/Edit/5
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

            var organizer = await _context.Organizer.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }
            return View(organizer);
        }

        // POST: Organizer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("id,email,password,name,isVerified")] Organizer organizer)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id != organizer.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizerExists(organizer.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Organizer Edited Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(organizer);
        }

        // GET: Organizer/Delete/5
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

            var organizer = await _context.Organizer
                .FirstOrDefaultAsync(m => m.id == id);
            if (organizer == null)
            {
                return NotFound();
            }

            return View(organizer);
        }

        // POST: Organizer/Delete/5
        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDeleteConfirmed(int id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var organizer = await _context.Organizer.FindAsync(id);
            _context.Organizer.Remove(organizer);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Organizer Deleted Successful";
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Verify Organizer
        public async Task<IActionResult> VerifyOrganizer(int id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var organizer = await _context.Organizer.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }
            organizer.isVerified = true;
            _context.Organizer.Update(organizer);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Organizer Verified Successful";
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Unverify Organizer
        public async Task<IActionResult> UnverifyOrganizer(int id)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            var organizer = await _context.Organizer.FindAsync(id);
            if (organizer == null)
            {
                return NotFound();
            }
            organizer.isVerified = false;
            _context.Organizer.Update(organizer);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Organizer Unverified Successful";
            return RedirectToAction(nameof(AdminIndex));
        }


        private bool OrganizerExists(int id)
        {
            return _context.Organizer.Any(e => e.id == id);
        }

        // is organizerLogin
        public bool isOrganizerLogin()
        {
            if (HttpContext.Session.GetInt32("organizerId") != null)
            {
                return true;
            }
            return false;
        }

        // is organizerVerified
        public bool isOrganizerVerified()
        {
            if (HttpContext.Session.GetInt32("organizerId") != null)
            {
                var organizerId = HttpContext.Session.GetInt32("organizerId");
                var organizer = _context.Organizer.Find(organizerId);
                if (organizer.isVerified)
                {
                    return true;
                }
            }
            return false;
        }

        // Organizer Index
        public async Task<IActionResult> Index()
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            var mVCSMS = _context.Seminar.Include(s => s.Organizer).Where(s => s.OrganizerId == organizerId && s.Seminar_Date >= DateTime.Now).OrderBy(s => s.Seminar_Date).ThenBy(s => s.Starting_Time);
            var organizer = _context.Organizer.Find(organizerId);
            ViewBag.organizer = organizer;
            ViewBag.organizerId = organizerId;
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            return View(await mVCSMS.ToListAsync());
        }

        // GET: CreateSeminar

    }
}
