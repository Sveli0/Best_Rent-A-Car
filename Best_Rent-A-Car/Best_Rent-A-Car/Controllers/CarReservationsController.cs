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
using Best_Rent_A_Car.Models.Attributes;
using System.Threading;

namespace Best_Rent_A_Car.Controllers
{
    public class CarReservationsController : Controller
    {
        /// <summary>
        /// A controller dealing with reservations
        /// </summary>

        private readonly ApplicationDbContext _context;

        public CarReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Shows a list for the admin, which contains reservations that have yet to be approved
        /// </summary>
        /// <returns></returns>
        // GET: CarReservations/PendingIndex
        public async Task<IActionResult> PendingIndex()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User).Where(c => c.Pending);
            return View(await applicationDbContext.ToListAsync());
        }
        /// <summary>
        /// Shows a list for the admins of all approved reservations
        /// </summary>
        /// <returns></returns>
        // GET: CarReservations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User).Where(c => !c.Pending);
            return View(await applicationDbContext.ToListAsync());
        }
        /// <summary>
        /// Shows a speciifc users reservation, used for the functionality of a user being able to see their own reservations
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// An enum to convert from the pending bool to an appropriate string
        /// </summary>
        /// <param name="pending">input boolean, coming from the database field of pending</param>
        /// <returns>returns "Approved"</returns>
        private static string PendingEnum(bool pending)
        {
            if (pending) return "Not Approved";
            else return "Appoved";
        }
        /// <summary>
        /// A view to contain shortened and concise data for reservations as well as the cars assigned to them
        /// </summary>
        public class ReservationIndexViewModel
        {
            public int carID { get; set; }
            [Display(Name = "Start Date")]
            [DataType(DataType.Date)]
            [DateAttribute]
            public DateTime StartDate { get; set; }
            [Display(Name = "End Date")]
            [DataType(DataType.Date)]
            [DateAttribute(ErrorMessage = "Date is past, or too far in the future.")]
            public DateTime EndDate { get; set; }
            public string Info { get; set; }
            public string Pending { get; set; }
            public double TotalAmount { get; set; } = 0;

        }
        /// <summary>
        /// A search method with the logic of using the input params to find all cars, which are not busy
        /// and then redirect the user to a make a reservation page which has all the available cars listed
        /// </summary>
        /// <param name="startDate"> startDate from the users query in the search view</param>
        /// <param name="endDate"> endDate from the users query in the search view</param>
        /// <returns></returns>
        // POST: CarReservations/Search
        [HttpPost]
        public IActionResult Search(DateTime startDate, DateTime endDate)
        {
            if (startDate<endDate&& startDate>DateTime.Now)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var nonUserQueryIds = _context.CarReservations
                    .Where(cr => cr.VisibleUserID != loggedInUserId)
                    .Select(cr => cr.CarID)
                    .ToList();
                var nonReservedCars = _context.Cars.Where(c => !_context.CarReservations.Select(cr => cr.CarID).ToList().Contains(c.Id)).ToList();
                var availableCarsList = _context.CarReservations
                    .Include(cr => cr.Car)
                    .Where(cr => cr.StartDate > endDate || cr.EndDate < startDate)
                    .Where(cr=>cr.VisibleUserID!=loggedInUserId)
                    .Select(c => new Car()
                    {
                        Id = c.Car.Id,
                        Brand = c.Car.Brand,
                        Model = c.Car.Model,
                        Info = c.Car.Info,
                        PricePerDay = c.Car.PricePerDay,
                        Seats = c.Car.Seats,
                        Year= c.Car.Year
                    }).ToList();
                foreach (var item in nonReservedCars)
                {
                    availableCarsList.Add(item);
                }
                var availableCarsInfoList = availableCarsList
                    .Select(c => new ReservationViewModel
                    {
                        carID = c.Id,
                        Info = $"{c.Brand} {c.Model} {c.Year} | Seats: {c.Seats} | Price Per Day: {c.PricePerDay}"
                    })
                    .ToList();



                var viewModel = new CreateViewModel
                {
                    CarReservation = new CarReservation(),
                    AvailableCars = availableCarsInfoList
                };

                ViewData["LoggedInUserId"] = loggedInUserId;

                ViewData["StartDate"] = startDate.ToString();

                ViewData["EndDate"] = startDate;

                viewModel.CarReservation.VisibleUserID = loggedInUserId;
                viewModel.CarReservation.EndDate = endDate;
                viewModel.CarReservation.StartDate = startDate;

                ViewData["Cars"] = new SelectList(_context.Cars
                    .OrderBy(x => x.Brand)
                    .ThenBy(x => x.Model)
                    .Where(x => availableCarsInfoList
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
        /// <summary>
        /// A view model for the reservations that has their compact info
        /// </summary>
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
        /// <summary>
        /// A method to list all the details of a certain request/reservation
        /// </summary>
        /// <param name="userId">part of the reservation PK</param>
        /// <param name="carId">part of the reservation PK</param>
        /// <returns></returns>
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
        // GET: CarReservations/DetailsPending/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsPending(string userId, int carId)
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
        /// <summary>
        /// A view Model for the car creation page to ensure that only the available cars are listed for the creation
        /// </summary>
        /// the CarReservation field is the one used by default by the creation view in order to get all the validations and align the fields for binding
        /// the avialable cars List is the list of all the cars to show in the available cars section in the Create view
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

        /// <summary>
        /// The httpGet method for the create view, while somewhat unused in the program it is a stable implementation to have
        /// </summary>
        /// <param name="carReservation"></param>
        /// <returns></returns>
        // GET: CarReservations/Create
        [Authorize(Roles ="Admin")]
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
        /// <summary>
        /// The HttpGet for the Reserve, it takes the createViewModel and hands it to the view, as well as adding some data
        /// to the viewdata.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
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
        /// <summary>
        /// The HttpPost for the Reserve action it adds the reservation to the context
        /// </summary>
        /// <param name="carReservation">a binded property for the ReserveView</param>
        /// <returns></returns>
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

                return RedirectToAction(nameof(UserReservations));
            }
            CreateViewModel viewModel = new CreateViewModel();
            return View("Search");
        }
        /// <summary>
        /// The httpGet for the edit, supplies it with the appropriate Reservation given the Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Binds the appropraite user through the view and edits the things that it has to edit idk figure it out
        /// </summary>
        /// <param name="carReservation"></param>
        /// <returns></returns>
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
                    if (!CarReservationExists(carReservation.CarID, carReservation.VisibleUserID))
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
        /// <summary>
        /// The HttpGet for the Delete action it finds a user through the supplied paramaters to send it into a deleteconfirmed
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// A decline method similar to the delete method except it is specifically applied to Pending reservations
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Deleting pending reservations is a team based choice, the functionality could be altered so that declined reservations
        /// are saved separately, but we found no purpose in doing this
        /// </summary>
        /// <param name="carReservation"></param>
        /// <returns></returns>
        // POST: CarReservations/Decline
        [HttpPost, ActionName("Decline")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineConfirmed([Bind("CarID,VisibleUserID")] CarReservation carReservation)
        {
            _context.CarReservations.Remove(carReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingIndex));
        }
        /// <summary>
        /// A confirm window for the delete that also binds the carReservation
        /// </summary>
        /// <param name="carReservation"></param>
        /// <returns></returns>
        // POST: CarReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("CarID,VisibleUserID")] CarReservation carReservation)
        {
            _context.CarReservations.Remove(carReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        /// <summary>
        /// The HttpGet for the accept it displays the details of the query and asks the user if it wants to accept
        /// the query
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="carId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This is the result of the clicking of the accept button in the View, it sets the query's 
        /// Pending property to true, thus making it a reservation
        /// </summary>
        /// <param name="carReservation"></param>
        /// <returns></returns>
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


        /// <summary>
        /// A regular exists method to check wheter or not a reservation exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns> true or false depending on if the reservation is actually available</returns>
        private bool CarReservationExists(int id, string userId)
        {
            return _context.CarReservations.Any(e => e.CarID == id&&e.VisibleUserID==userId);
        }
    }
}
