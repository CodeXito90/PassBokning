using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PassBokning.Data;
using PassBokning.Models;

namespace PassBokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Bookings feature
        [Authorize]  // Only logged in users can book this feature
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            // Get the current user
            var userId = _userManager.GetUserId(User);
            if (userId == null) return NotFound();

            // Get the gym class including its attending members
            var gymClass = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gymClass == null) return NotFound();

            // Find if user is already booked
            var attending = gymClass.AttendingMembers
                .FirstOrDefault(a => a.ApplicationUserId == userId);

            if (attending == null)
            {
                // User is not booked - add booking
                var booking = new ApplicationUserGymClass
                {
                    GymClassId = gymClass.Id,
                    ApplicationUserId = userId
                };
                gymClass.AttendingMembers.Add(booking);
            }
            else
            {
                // User is already booked - remove booking
                gymClass.AttendingMembers.Remove(attending);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    
    // GET: GymClasses
    public async Task<IActionResult> Index()
        {
            return View(await _context.GymClasses.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(gymClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Add this temporarily for debugging
                    ModelState.AddModelError(string.Empty, "Error saving to database: " + ex.Message);
                }
            }

            // Add this temporarily for debugging
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
            }

            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }
    }
}
