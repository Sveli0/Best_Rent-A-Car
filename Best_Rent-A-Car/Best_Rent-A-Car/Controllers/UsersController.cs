using Best_Rent_A_Car.Data;
using Best_Rent_A_Car.Models;
using Best_Rent_A_Car.Models.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Best_Rent_A_Car.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }


        // GET: UserController/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: UserController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditViewModel model = new EditViewModel();
            model.User = user;
            return View(model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,string password, [Bind("Id,EGN,PasswordHash,Email,UserName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                
                try
                {
                    User user2 = await _context.Users.FindAsync(id);
                    user2.EGN = user.EGN;
                    user2.Email = user.Email;
                    user2.UserName = user.UserName;
                    if (password != null)
                    {
                        string newPassword = _userManager.PasswordHasher.HashPassword(user, password);
                        user2.PasswordHash = newPassword;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            return View(user);
        }

        // GET: UserController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }

        // POST: UserController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public class EditViewModel
        {
            public User User { get; set; }
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [Required]
            [EmailAddress]
            [UniqueEmail]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MinLength(10, ErrorMessage = "EGN must be 10 symbols long.")]
            [MaxLength(10, ErrorMessage = "EGN must be 10 symbols long.")]
            [Display(Name = "EGN")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "EGN must contain only numbers.")]
            [UniqueEGN]
            public string EGN { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            [Display(Name = "Username")]
            public string UserName { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            [MinLength(7, ErrorMessage = "Phone number must between 7-15 symbols long.")]
            [MaxLength(15, ErrorMessage = "Phone number must between 7-15 symbols long.")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers.")]
            public string PhoneNumber { get; set; }
        }
    }
}
