using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Best_Rent_A_Car.Data;
using Best_Rent_A_Car.Models;
using Microsoft.AspNetCore.Authorization;

namespace Best_Rent_A_Car.Controllers
{
    public class CarsController : Controller
    {
        /// <summary>
        /// This is an mvc class mostly generated through the wonders of razor and asp, still it hold some great methods
        /// </summary>
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Returns a collection of cars to the Index View so it can show them properly
        /// </summary>
        /// <returns></returns>
        // GET: Cars
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cars.ToListAsync());
        }
        /// <summary>
        /// This is an HttpGet method, Gets the details of a car through a provided id and gives it to the detail view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Cars/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
        /// <summary>
        /// The HttpGet method that invokes the create View
        /// </summary>
        /// <returns></returns>
        // GET: Cars/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// The HttpPost action for creating cars, it binds the inputs to the appropriate fields
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,Year,Seats,Info,PricePerDay")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }
        /// <summary>
        /// HttpGet for the Edit action, it supplies the view with the details it needs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Cars/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }
        /// <summary>
        /// The HttpPost action for the Edit, it takes the car and updates it within the context, with the new Binded fields
        /// </summary>
        /// <param name="id"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Year,Seats,Info,PricePerDay")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            return View(car);
        }
        /// <summary>
        /// The HttpGet for the DeleteView, it supplies the deletConfirm view with the necessary details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Cars/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
        /// <summary>
        /// The HttpPost for the deleteconfirm which happens when the delete button is pressed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// A simple method to show that the car with the given paramater existssa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
