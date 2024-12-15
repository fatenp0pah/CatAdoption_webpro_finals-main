using Microsoft.AspNetCore.Mvc;
using CatAdoption.Models;
using CatAdoption.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace CatAdoption.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Render the login page
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            ViewData["HideNavbar"] = true;
            return View();
        }

        // Handle login submissions
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Username and Password are required.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Add User ID
                    new Claim(ClaimTypes.Role, user.Role) // Add User Role
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["SuccessMessage"] = $"Login successful! Welcome, {user.Username}.";

                if (user.Role == "Admin")
                {
                    // Redirect admin to the admin dashboard
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    // Redirect regular users to the home page
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password. Please try again.";
                return View();
            }
        }

        // Handle logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // Render the registration page
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            ViewData["HideNavbar"] = true; // Hide navbar on register page
            return View();
        }

        // Handle registration submissions
        [HttpPost]
        public IActionResult Register(string Username, string Password, string ConfirmPassword, string Email)
        {
            // Ensure all required fields are provided
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) || string.IsNullOrEmpty(Email))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return View();
            }

            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Password and Confirm Password do not match.";
                return View();
            }

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == Username))
            {
                TempData["ErrorMessage"] = "Username is already taken. Please choose another.";
                return View();
            }

            // Check if email is already registered
            if (_context.Users.Any(u => u.Email == Email))
            {
                TempData["ErrorMessage"] = "Email is already registered. Please use another.";
                return View();
            }

            // Create new user
            var newUser = new User
            {
                Username = Username,
                Password = Password, // Password is stored as plain text (NOT recommended for production)
                Email = Email,
                Role = "Customer", // Assign default role
                CreatedAt = DateTime.Now
            };

            // Add user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }
    }
}
