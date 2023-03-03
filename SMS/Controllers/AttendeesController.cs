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
    public class AttendeesController : Controller
    {
        private readonly MVCSMS _context;

        public AttendeesController(MVCSMS context)
        {
            _context = context;
        }

        // is user login
        public bool IsUserLogin()
        {
            if (HttpContext.Session.GetInt32("userId") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: seminar list that attendee is registered to
        public async Task<IActionResult> SeminarHistory()
        {
            if (!IsUserLogin()){
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "You must be logged in to view this page";
                    return RedirectToAction("Login", "Home");
            }
            var userId = HttpContext.Session.GetInt32("userId");
            var user = _context.Person.FindAsync(userId);
            var mVCSMS = _context.Registration.Include(r => r.attendee).Include(r => r.seminar).Include(r => r.seminar.Organizer).Where(r=>r.attendeeId == userId).OrderByDescending(s => s.seminar.Seminar_Date).ThenBy(s => s.seminar.Starting_Time);
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            ViewBag.userId = userId;
            return View(await mVCSMS.ToListAsync());
        }

        // GET: All Upcoming Seminars that attendee is not registered to
        public async Task<IActionResult> UpcomingSeminars()
        {
            if (!IsUserLogin()){
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "You must be logged in to view this page";
                    return RedirectToAction("Login", "Home");
            }
            var userId = HttpContext.Session.GetInt32("userId");
            var user = _context.Person.FindAsync(userId);
            var seminar = await _context.Seminar.Include(r => r.Organizer).Where(s => s.Seminar_Date >= DateTime.Now).OrderBy(s => s.Seminar_Date).ThenBy(s => s.Starting_Time).ToListAsync();
            // Exclude seminars that are registered by userId
            var mVCSMS = _context.Registration.Include(r => r.attendee).Include(r => r.seminar).Include(r => r.seminar.Organizer).Where(r=>r.attendeeId == userId);
            foreach (var s in mVCSMS)
            {
                seminar.Remove(seminar.Find(x => x.id == s.seminarId));
            }
            ViewBag.messageClass = TempData["messageClass"];
            ViewBag.message = TempData["message"];
            ViewBag.userId = userId;
            return View(seminar);
        }

        // GET: Register to seminar
        public async Task<IActionResult> RegisterSeminar(int? id)
        {
            if (!IsUserLogin()){
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "You must be logged in to view this page";
                    return RedirectToAction("Login", "Home");
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
            var userId = HttpContext.Session.GetInt32("userId");
            var user = await _context.Person.FindAsync(userId);
            var isRegistered = _context.Registration.Where(r => r.attendeeId == userId && r.seminarId == seminar.id).FirstOrDefault();
            if (isRegistered != null)
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You are already registered for this seminar";
                return RedirectToAction("UpcomingSeminars");
            }
            Registration registration = new Registration();
            registration.attendeeId = user.id;
            registration.seminarId = seminar.id;
            _context.Add(registration);
            await _context.SaveChangesAsync();
            TempData["messageClass"] ="alert alert-success";
            TempData["message"] = "You have successfully registered for this seminar";
            ViewBag.userId = userId;
            return RedirectToAction("UpcomingSeminars");
        }

        // GET: Unregister to seminar
        public async Task<IActionResult> UnregisterSeminar(int? id)
        {
            if (!IsUserLogin()){
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "You must be logged in to view this page";
                    return RedirectToAction("Login", "Home");
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
            var userId = HttpContext.Session.GetInt32("userId");
            var user = await _context.Person.FindAsync(userId);
            var isRegistered = _context.Registration.Where(r => r.attendeeId == userId && r.seminarId == id).FirstOrDefault();
            if (isRegistered == null)
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You are not registered for this seminar";
                return RedirectToAction("Index", "Home");
            }
            _context.Registration.Remove(isRegistered);
            await _context.SaveChangesAsync();
            TempData["messageClass"] ="alert alert-success";
            TempData["message"] = "You have successfully unregistered for this seminar";
            ViewBag.userId = userId;
            return RedirectToAction("Index", "Home");
        }

        // GET: View seminar details
        public async Task<IActionResult> ViewSeminarDetails(int? id)
        {
            if (!IsUserLogin()){
                    TempData["messageClass"] ="alert alert-danger";
                    TempData["message"] = "You must be logged in to view this page";
                    return RedirectToAction("Login", "Home");
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
            var userId = HttpContext.Session.GetInt32("userId");
            var user = await _context.Person.FindAsync(userId);
            var isRegistered = _context.Registration.Where(r => r.attendeeId == userId && r.seminarId == id).FirstOrDefault();
            ViewBag.isRegistered = isRegistered;
            ViewBag.userId = userId;
            return View(seminar);
        }

    }
}