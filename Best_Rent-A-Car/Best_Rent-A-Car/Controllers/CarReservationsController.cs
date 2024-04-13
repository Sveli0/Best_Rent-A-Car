using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Best_Rent_A_Car.Data;
using Best_Rent_A_Car.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: CarReservations/Search
        [HttpPost]
        public IActionResult Search(DateTime startDate, DateTime endDate)
        {
            var list = _context.CarReservations.Include(c => c.Car).Where(x => x.StartDate > endDate || x.EndDate < startDate);


            var list1 = list.Select(c => new ReservationViewModel
            {
                carID = c.CarID,
                Year = c.Car.Year,
                Info = $"{c.Car.Brand} {c.Car.Model} Seats: {c.Car.Seats} Price Per Day: {c.Car.PricePerDay}"
            });


            var viewModel = new CreateViewModel()
            {
                CarReservation = new CarReservation(),
                AvailableCars = list1.ToList()
            };

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            viewModel.CarReservation.VisibleUserID = loggedInUserId;
            viewModel.EndDate = endDate;
            viewModel.StartDate = startDate;
           viewModel.carID = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel"); ;

            return View("CreateSearch", viewModel);



        }
        public class ReservationViewModel
        {
            public int carID { get; set; }
            public int Year { get; set; }
            public string Info { get; set; }

        }
        // GET: CarReservations/Search
        // TODO: Authorize
        public IActionResult Search()
        {
            return View("Search");
        }

        // GET: CarReservations/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string userId, int carId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .Include(c => c.Car)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.VisibleUserID == userId && m.CarID == carId);
            if (carReservation == null)
            {
                return NotFound();
            }
            
            return View(carReservation);
        }

        public class CreateViewModel
        {
            public CarReservation CarReservation { get; set; }
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }
            public List<ReservationViewModel> AvailableCars { get; set; }
            public SelectList carID { get; set; }

            public CreateViewModel()
            {
                CarReservation = new CarReservation();
                AvailableCars = new List<ReservationViewModel>();
            }
        }


        // GET: CarReservations/Create
        [Authorize]
        public IActionResult Create(List<ReservationViewModel> availableCars)
        {

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["LoggedInUserId"] = loggedInUserId;


            ViewData["Cars"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel"); ;
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");
            return View(availableCars);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("CarReservations/CreateSearch")]
        public IActionResult CreateSearch(CreateViewModel viewModel)
        {

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["LoggedInUserId"] = loggedInUserId;


            ViewData["Cars"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel"); ;
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");

            return View("Create", viewModel);
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

                Random r = new Random();
                int a = r.Next(10);
                if (a == 6)
                {

                    return Redirect("https://www.doyou.com/wp-content/uploads/2021/01/15-i-have-no-idea.jpg");
                }
                _context.Add(carReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(carReservation);
        }

        // POST: CarReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSearch([Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carReservation);
                await _context.SaveChangesAsync();
                Random r = new Random();
                int a = r.Next(10);
                if (a == 6)
                {

                    return Redirect("https://www.doyou.com/wp-content/uploads/2021/01/15-i-have-no-idea.jpg");
                }

                return RedirectToAction(nameof(Create));
            }
            return View(carReservation);
        }

        // GET: CarReservations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string userId, int carId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .FirstOrDefaultAsync(m => m.CarID == carId && m.VisibleUserID == userId);

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
        public async Task<IActionResult> Edit([Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userId, int carId)
        {
            if (userId == null)
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
        public async Task<IActionResult> DeleteConfirmed([Bind("CarID,VisibleUserID")] CarReservation carReservation)
        {
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
