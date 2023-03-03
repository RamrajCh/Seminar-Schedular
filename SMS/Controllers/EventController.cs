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
    public class EventController : Controller
    {
        private readonly MVCSMS _context;

        public EventController(MVCSMS context)
        {
            _context = context;
        }

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

        // GET: View my seminars
        public async Task<IActionResult> MySeminars()
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            var mVCSMS = _context.Seminar.Include(s => s.Organizer).Where(s => s.OrganizerId == organizerId).OrderByDescending(s => s.Seminar_Date).ThenBy(s => s.Starting_Time);
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            ViewBag.organizerId = organizerId;
            return View(await mVCSMS.ToListAsync());
        }


        // GET: View Seminar Details
        public async Task<IActionResult> ViewSeminarDetails(int? id)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
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
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (seminar.OrganizerId != organizerId)
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You are not authorized to view this page";
                return RedirectToAction("MySeminars", "Event");
            }
            // get all registrations for this seminar
            var registrations = _context.Registration.Where(r => r.seminarId == id).Include(r => r.attendee).Include(r => r.seminar).OrderBy(r => r.attendee.name).ToListAsync();
            ViewBag.registrations = registrations;
            ViewBag.organizerId = HttpContext.Session.GetInt32("organizerId");
            return View(seminar);
        }


        // GET: Create Event
        public IActionResult CreateNewSeminar()
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
            }
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            ViewBag.organizerId = HttpContext.Session.GetInt32("organizerId");
            return View();
        }

        // POST: Create Event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewSeminar([Bind("id,topic,platform,link,linkId,password,type,Seminar_Date,Starting_Time,Ending_Time")] Seminar seminar)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
            }
            if (ModelState.IsValid)
            {
                var organizerId = HttpContext.Session.GetInt32("organizerId");
                var organizer = _context.Organizer.Find(organizerId);
                seminar.Organizer = organizer;
                _context.Add(seminar);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Seminar created successfully";
                return RedirectToAction("Index", "Organizer");
            }
            ViewBag.organizerId = HttpContext.Session.GetInt32("organizerId");
            return View(seminar);
        }

        // GET: Edit Event
        public async Task<IActionResult> EditSeminar(int? id)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
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
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (seminar.Organizer.id != organizerId)
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You are not authorized to edit this seminar";
                return RedirectToAction("Index", "Organizer");
            }
            ViewBag.organizerId = organizerId;
            return View(seminar);
        }

        // POST: Edit Event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSeminar(int id, [Bind("id,topic,platform,link,linkId,password,type,Seminar_Date,Starting_Time,Ending_Time")] Seminar seminar)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
            }
            if (id != seminar.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var organizerId = HttpContext.Session.GetInt32("organizerId");
                    var organizer = _context.Organizer.Find(organizerId);
                    seminar.Organizer = organizer;
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
                return RedirectToAction("Index", "Organizer");
            }
            ViewBag.organizerId = HttpContext.Session.GetInt32("organizerId");
            return View(seminar);
        }

        // GET: Delete Event
        public async Task<IActionResult> DeleteSeminar(int? id)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
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
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (seminar.Organizer.id != organizerId)
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You are not authorized to delete this seminar";
                return RedirectToAction("Index", "Organizer");
            }
            ViewBag.organizerId = organizerId;
            return View(seminar);
        }

        // POST: Delete Event
        [HttpPost, ActionName("DeleteSeminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSeminarConfirmed(int id)
        {
            if (!isOrganizerLogin())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginOrganizer", "Home");
            }
            if (!isOrganizerVerified())
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You must be verified to view this page";
                return RedirectToAction("Index", "Organizer");
            }
            var seminar = await _context.Seminar.FindAsync(id);
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (seminar.Organizer.id != organizerId)
            {
                TempData["messageClass"] = "alert alert-danger";
                TempData["message"] = "You are not authorized to delete this seminar";
                return RedirectToAction("Index", "Organizer");
            }
            _context.Seminar.Remove(seminar);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Seminar Deleted Successful";
            return RedirectToAction("Index", "Organizer");
        }

        private bool SeminarExists(int id)
        {
            return _context.Seminar.Any(e => e.id == id);
        }

    }

}