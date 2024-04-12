using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Best_Rent_A_Car.Data;
using Best_Rent_A_Car.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace Best_Rent_A_Car.Controllers
{
    public class CarReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarReservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: CarReservations/Search
        [HttpPost]
        public async Task<IActionResult> Search(DateTime startDate, DateTime endDate)
        {
            var list = _context.CarReservations.Include(c => c.Car).Where(x=>x.StartDate>endDate||x.EndDate<startDate);
                
            
            var list1 = list.Select(c => new CarViewModel
            {
                Brand = c.Car.Brand,
                Model = c.Car.Model,
                Seats = c.Car.Seats,
                PricePerDay = c.Car.PricePerDay,
                Year = c.Car.Year,
                Info = c.Car.Info
            });

            return RedirectToAction(nameof(Create), await list1.ToListAsync());
        }
        public class CarViewModel
        {
            public string Brand { get; set; }

            public string Model { get; set; }

            public int Seats { get; set; }
            public double PricePerDay { get; set; }
            public int Year { get; set; }
            public string Info { get; set; }
        }
        // GET: CarReservations/Search
        public IActionResult Search()
        {
            return View();
        }

        // GET: CarReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .Include(c => c.Car)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (carReservation == null)
            {
                return NotFound();
            }

            return View(carReservation);
        }

        public class CreateViewModel
        {
            public CarReservation CarReservation { get; set; }
            public List<CarViewModel> AvailableCars { get; set; } 
        }


        // GET: CarReservations/Create
        public IActionResult Create(List<CarViewModel> availableCars)
        {
            var viewModel = new CreateViewModel
            {
                CarReservation = new CarReservation(),
                AvailableCars = availableCars
            };


            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["LoggedInUserId"] = loggedInUserId;

            ViewData["Cars"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel"); ;
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");
            return View(viewModel);
        }

        // POST: CarReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
            if (ModelState.IsValid)
            {

                _context.Add(carReservation);
                await _context.SaveChangesAsync();
                Random r = new Random();
                int a = r.Next(10);
                if (a==6)
                {

                return Redirect("https://www.doyou.com/wp-content/uploads/2021/01/15-i-have-no-idea.jpg");
                }
                return RedirectToAction(nameof(Create));
            }
            return View(carReservation);
        }

        // GET: CarReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations.FindAsync(id);
            if (carReservation == null)
            {
                return NotFound();
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "Id", "Brand", carReservation.CarID);
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id", carReservation.VisibleUserID);
            return View(carReservation);
        }

        // POST: CarReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
            if (id != carReservation.CarID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarReservationExists(carReservation.CarID))
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
            ViewData["CarID"] = new SelectList(_context.Cars, "Id", "Brand", carReservation.CarID);
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id", carReservation.VisibleUserID);
            return View(carReservation);
        }

        // GET: CarReservations/Delete/5
        public async Task<IActionResult> Delete(string userId, int carId)
        {
            if (userId == null || carId == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .Include(c => c.Car)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CarID == carId && m.VisibleUserID == userId);

            if (carReservation == null)
            {
                return NotFound();
            }

            return View(carReservation);
        }

        // POST: CarReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId, int carId)
        {
            var carReservation = await _context.CarReservations
                .FirstOrDefaultAsync(m => m.CarID == carId && m.VisibleUserID == userId);
            _context.CarReservations.Remove(carReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarReservationExists(int id)
        {
            return _context.CarReservations.Any(e => e.CarID == id);
        }
    }
}
