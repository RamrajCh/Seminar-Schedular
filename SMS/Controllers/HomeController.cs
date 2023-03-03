#nullable disable
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
    public class HomeController : Controller
    {
        private readonly MVCSMS _context;

        public HomeController(MVCSMS context)
        {
            _context = context;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            
            if (userId != null)
            {
                var user = _context.Person.Where(p => p.id == userId).FirstOrDefault();
                var mVCSMS = _context.Registration.Include(r => r.attendee).Include(r => r.seminar).Include(r => r.seminar.Organizer).Where(r=>r.attendeeId == userId && r.seminar.Seminar_Date >= DateTime.Now).OrderBy(s => s.seminar.Seminar_Date).ThenBy(s => s.seminar.Starting_Time);
                ViewBag.user = user;
                ViewBag.messageClass = TempData["messageClass"]?.ToString();
                ViewBag.message = TempData["message"]?.ToString();
                ViewBag.userId = userId;
                return View(await mVCSMS.ToListAsync());
            }
            else if (HttpContext.Session.GetInt32("organizerId") != null)
            {
                return RedirectToAction("Index", "Organizer");
            }
            else if (HttpContext.Session.GetInt32("adminId") != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Welcome");
            }
        }
        
        public ActionResult Welcome()
        {
            return View();
        }

        // GET: Register
        public ActionResult Register()
        {
            ViewBag.messageClass = TempData["messageClass"]?.ToString();
            ViewBag.message = TempData["message"]?.ToString();
            return View();
        }
        

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Person _user)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Person.Where(x => x.koiId == _user.koiId || x.email == _user.email).FirstOrDefault();
                if (check == null)
                {
                    _user.password = HashPassword(_user.password);
                    _context.Person.Add(_user);
                    _context.SaveChanges();
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Registration Successful";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Koi ID or Email already exists";
                    return View();
                }
            }
            return View();
        }

        // GET: Login
        public ActionResult Login()
        {
            ViewBag.messageClass = TempData["messageClass"]?.ToString();
            ViewBag.message = TempData["message"]?.ToString();
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = HashPassword(password);
                var user = _context.Person.Where(x => (x.koiId == username || x.email == username) && x.password == f_password).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.SetInt32("userId", user.id);
                    HttpContext.Session.SetString("koiID", user.koiId);
                    HttpContext.Session.SetString("name", user.name);
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Login Successful";
                    return RedirectToAction("Index");
                }
                else
                {   
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Login failed! Your KOI Id / Email or Password is incorrect!";
                    return View();
                }
            }
            return View();
        }

        // GET: Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Welcome");
        }

        // Hash password
        public static string HashPassword(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i=0; i<targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }

        //GET: password change
        public ActionResult ChangePassword(){
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null){
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "You must be logged in to change password!";
                return RedirectToAction("Login");
            }
            return View();
        }

        //POST: password change
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword){
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null){
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "You must be logged in to change password!";
                return RedirectToAction("Login");
            }
            var user = _context.Person.Where(x => x.id == userId).FirstOrDefault();
            if (user != null)
            {
                var oldPasswordHash = HashPassword(oldPassword);
                if (oldPasswordHash == user.password)
                {
                    user.password = HashPassword(newPassword);
                    _context.SaveChanges();
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Password changed successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Old password is incorrect!";
                    return View();
                }
            }
            return View();
        }

        // GET: Admin Login
        public ActionResult LoginAdmin()
        {
            ViewBag.messageClass = TempData["messageClass"]?.ToString();
            ViewBag.message = TempData["message"]?.ToString();
            return View();
        }

        // POST: Admin Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var admin = _context.Admin.Where(a => a.username == username && a.password == password).FirstOrDefault();
                if (admin != null)
                {
                    HttpContext.Session.SetInt32("adminId", admin.id);
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Login Successful";
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Login failed! Your username or Password is incorrect!";
                    return View();
                }
            }
            return View();
        }

        // GET: Organizer Register
        public ActionResult RegisterOrganizer()
        {
            ViewBag.messageClass = TempData["messageClass"]?.ToString();
            ViewBag.message = TempData["message"]?.ToString();
            return View();
        }

        // POST: Organizer Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterOrganizer(Organizer _organizer)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Organizer.Where(x => x.email == _organizer.email).FirstOrDefault();
                if (check == null)
                {
                    _organizer.password = HashPassword(_organizer.password);
                    _context.Organizer.Add(_organizer);
                    _context.SaveChanges();
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Registration Successful";
                    return RedirectToAction("LoginOrganizer");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Email already exists";
                    return View();
                }
            }
            ViewBag.messageClass = "alert alert-danger";
            ViewBag.message = "Registration Failed";
            return View();
        }

        // GET: Organizer Login
        public ActionResult LoginOrganizer()
        {
            ViewBag.messageClass = TempData["messageClass"]?.ToString();
            ViewBag.message = TempData["message"]?.ToString();
            return View();
        }

        // POST: Organizer Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginOrganizer(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = HashPassword(password);
                var organizer = _context.Organizer.Where(o => o.email == email && o.password == f_password).FirstOrDefault();
                if (organizer != null)
                {
                    HttpContext.Session.SetInt32("organizerId", organizer.id);
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Login Successful";
                    return RedirectToAction("Index", "Organizer");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Login failed! Your email or Password is incorrect!";
                    return View();
                }
            }
            return View();
        }

        // GET: Organizer password change
        public ActionResult ChangeOrganizerPassword()
        {
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (organizerId == null)
            {
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "You must be logged in to change password!";
                return RedirectToAction("LoginOrganizer");
            }
            return View();
        }

        // POST: Organizer password change
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeOrganizerPassword(string oldPassword, string newPassword)
        {
            var organizerId = HttpContext.Session.GetInt32("organizerId");
            if (organizerId == null)
            {
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "You must be logged in to change password!";
                return RedirectToAction("LoginOrganizer");
            }
            var organizer = _context.Organizer.Where(x => x.id == organizerId).FirstOrDefault();
            if (organizer != null)
            {
                var oldPasswordHash = HashPassword(oldPassword);
                if (oldPasswordHash == organizer.password)
                {
                    organizer.password = HashPassword(newPassword);
                    _context.SaveChanges();
                    TempData["messageClass"] = "alert alert-success";
                    TempData["message"] = "Password changed successfully!";
                    return RedirectToAction("Index", "Organizer");
                }
                else
                {
                    ViewBag.messageClass = "alert alert-danger";
                    ViewBag.message = "Old password is incorrect!";
                    return View();
                }
            }
            return View();
        }
    }
}
