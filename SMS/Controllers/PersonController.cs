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
    public class PersonController : Controller
    {
        private readonly MVCSMS _context;

        public PersonController(MVCSMS context)
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

        // GET: Person
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
            return View(await _context.Person.ToListAsync());
        }

        // GET: Person/Details/5
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

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Person/Create
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

        // POST: Person/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("id,email,name,koiId,password,faculty,role,joinedDate")] Person person)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Person Created Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(person);
        }

        // GET: Person/Edit/5
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

            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("id,email,name,koiId,password,faculty,role,joinedDate")] Person person)
        {
            if (!isAdminLogin())
            {
                TempData["messageClass"] ="alert alert-danger";
                TempData["message"] = "You must be logged in to view this page";
                return RedirectToAction("LoginAdmin", "Home");
            }
            if (id != person.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["messageClass"] = "alert alert-success";
                TempData["message"] = "Person Edited Successful";
                return RedirectToAction(nameof(AdminIndex));
            }
            return View(person);
        }

        // GET: Person/Delete/5
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

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Person/Delete/5
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
            var person = await _context.Person.FindAsync(id);
            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
            TempData["messageClass"] = "alert alert-success";
            TempData["message"] = "Person Deleted Successful";
            return RedirectToAction(nameof(AdminIndex));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.id == id);
        }
    }
}
