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
using PassBokning.Models.ViewModels;

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

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var gymClasses = await _context.GymClasses
                .Include(g => g.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .Where(g => g.StartTime > DateTime.Now)
                .ToListAsync();

            var model = gymClasses.Select(g => new IndexGymClassViewModel
            {
                Id = g.Id,
                Name = g.Name,
                StartTime = g.StartTime,
                Duration = g.Duration,
                Description = g.Description,
                Attending = g.AttendingMembers.Any(a => a.ApplicationUserId == userId)

            }).ToList();


            return View(model);
        }

        //Bookings feature
        [Authorize(Roles ="Admin")]  // Only logged in users can book this feature
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
                // If the user is not booked - add booking
                var booking = new ApplicationUserGymClass
                {
                    GymClassId = gymClass.Id,
                    ApplicationUserId = userId
                };
                gymClass.AttendingMembers.Add(booking);
            }
            else
            {
                // remove booking
                gymClass.AttendingMembers.Remove(attending);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> History()
        {
            var gymClasses = await _context.GymClasses
                .Where(g => g.StartTime < DateTime.Now)
                .ToListAsync();

            var model = gymClasses.Select(g => new IndexGymClassViewModel
            {
                Id = g.Id,
                Name = g.Name,
                StartTime = g.StartTime,
                Duration = g.Duration,
                Description = g.Description
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);

            var bookedClasses = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.AttendedClasses)
                .Select(ac => ac.GymClass)
                .Where(g => g.StartTime > DateTime.Now)
                .ToListAsync();

            var model = bookedClasses.Select(b => new IndexGymClassViewModel
            {
                Id = b.Id,
                Name = b.Name,
                StartTime = b.StartTime,
                Duration = b.Duration,
                Description = b.Description
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> MyHistory()
        {
            var userId = _userManager.GetUserId(User);

            var historicalClasses = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.AttendedClasses)
                .Select(ac => ac.GymClass)
                .Where(g => g.StartTime < DateTime.Now)
                .ToListAsync();

            var model = historicalClasses.Select(g => new IndexGymClassViewModel
            {
                Id = g.Id,
                Name = g.Name,
                StartTime = g.StartTime,
                Duration = g.Duration,
                Description = g.Description
            }).ToList();

            return View(model);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClassWithAttendees = await _context.GymClasses
                .Where(g => g.Id == id)
                .Include(c => c.AttendingMembers)
                .ThenInclude(u => u.ApplicationUser).FirstOrDefaultAsync();

            if (gymClassWithAttendees == null)
            {
                return NotFound();
            }

            return View(gymClassWithAttendees);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateGymClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var gymClass = new GymClass
                {
                    Name = viewModel.Name,
                    StartTime = viewModel.StartTime,
                    Duration = viewModel.Duration,
                    Description = viewModel.Description
                };

                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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