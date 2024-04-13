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


        // GET: CarReservations/PendingIndex

        public async Task<IActionResult> PendingIndex()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User).Where(c => c.Pending);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: CarReservations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User).Where(c => !c.Pending);
            return View(await applicationDbContext.ToListAsync());
        }
        //Get: CarReservations/UserReservations
        [Authorize]
        public async Task<IActionResult> UserReservations()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userOrders = _context.CarReservations.Include(c => c.Car).Where(x => x.VisibleUserID == loggedInUserId);

            var userOrdersModels = userOrders.Select(c => new ReservationIndexViewModel
            {
                carID = c.CarID,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                TotalAmount = (c.EndDate - c.StartDate).TotalDays * c.Car.PricePerDay,
                Pending = PendingEnum(c.Pending),
                Info = $"{c.Car.Brand} {c.Car.Model} {c.Car.Year} | Seats: {c.Car.Seats} | Price Per Day: {c.Car.PricePerDay}"

            });

            return View(await userOrdersModels.ToListAsync());
        }
        private static string PendingEnum(bool pending)
        {
            if (pending) return "Not Approved";
            else return "Appoved";
        }
        public class ReservationIndexViewModel
        {
            public int carID { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }
            public string Info { get; set; }
            public string Pending { get; set; }
            public double TotalAmount { get; set; } = 0;

        }

        // POST: CarReservations/Search
        [HttpPost]
        public IActionResult Search(DateTime startDate, DateTime endDate)
        {
            if (startDate<endDate&& endDate>DateTime.Now)
            {
                var availableCarsQuery = _context.Cars
                    .Where(c => !_context.CarReservations
                    .Any(cr => cr.CarID == c.Id &&
                    (cr.StartDate <= endDate && cr.EndDate >= startDate)));

                var availableCarsList = availableCarsQuery
                    .Select(c => new ReservationViewModel
                    {
                        carID = c.Id,
                        Info = $"{c.Brand} {c.Model} {c.Year} | Seats: {c.Seats} | Price Per Day: {c.PricePerDay}"
                    })
                    .ToList();

                var viewModel = new CreateViewModel
                {
                    CarReservation = new CarReservation(),
                    AvailableCars = availableCarsList
                };

                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewData["LoggedInUserId"] = loggedInUserId;

                ViewData["StartDate"] = startDate.ToString();

                ViewData["EndDate"] = startDate;

                viewModel.CarReservation.VisibleUserID = loggedInUserId;
                viewModel.CarReservation.EndDate = endDate;
                viewModel.CarReservation.StartDate = startDate;

                ViewData["Cars"] = new SelectList(_context.Cars
                    .OrderBy(x => x.Brand)
                    .ThenBy(x => x.Model)
                    .Where(x => availableCarsList
                    .Select(c => c.carID)
                    .ToList()
                    .Contains(x.Id))
                    .Select(c => new
                    {
                        Id = c.Id,
                        FullBrandAndModel = $"{c.Brand} {c.Model}"
                    }), "Id", "FullBrandAndModel");

                return View("Reserve", viewModel);
            }
            return View("Search");
        }
        public class ReservationViewModel
        {
            public int carID { get; set; }
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
            public List<ReservationViewModel> AvailableCars { get; set; }
            public CreateViewModel()
            {
                CarReservation = new CarReservation();
                AvailableCars = new List<ReservationViewModel>();
            }
        }


        // GET: CarReservations/Create
        [Authorize]
        public IActionResult Create(CarReservation carReservation)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["LoggedInUserId"] = loggedInUserId;

            ViewData["Cars"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel");

            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");
            return View(carReservation);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("CarReservations/Reserve")]
        public IActionResult Reserve(CreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewData["LoggedInUserId"] = loggedInUserId;

                ViewData["Cars"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
                {
                    Id = c.Id,
                    FullBrandAndModel = $"{c.Brand} {c.Model}"
                }), "Id", "FullBrandAndModel");

                ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");

                return View("Create", viewModel);
            }
            return View("Search");
        }

        // POST: CarReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve([Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
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

                return RedirectToAction(nameof(UserReservations));
            }
            CreateViewModel viewModel = new CreateViewModel();
            return View("Search");
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

        // GET: CarReservations/Decline/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Decline(string userId, int carId)
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

        // POST: CarReservations/Decline
        [HttpPost, ActionName("Decline")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineConfirmed([Bind("CarID,VisibleUserID")] CarReservation carReservation)
        {
            _context.CarReservations.Remove(carReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingIndex));
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

        // GET: CarReservations/Accept/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept(string userId, int carId)
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

        // POST: CarReservations/Accept
        [HttpPost, ActionName("Accept")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptConfirm([Bind("CarID,VisibleUserID")] CarReservation carReservation)
        {
            CarReservation contextReservation = await _context.CarReservations
                .FirstOrDefaultAsync(m => m.CarID == carReservation.CarID && m.VisibleUserID == carReservation.VisibleUserID);
            contextReservation.Pending = 1 == 2;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingIndex));
        }



        private bool CarReservationExists(int id)
        {
            return _context.CarReservations.Any(e => e.CarID == id);
        }
    }
}
