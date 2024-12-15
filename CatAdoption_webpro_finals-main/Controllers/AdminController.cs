using CatAdoption.Data;
using CatAdoption.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CatAdoption.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard
        public IActionResult Dashboard()
        {
            // Get all cats and adoption requests, including user and cat details
            var cats = _context.Cats.ToList();
            var adoptions = _context.Adoptions.Include(a => a.User).Include(a => a.Cat).ToList();

            ViewBag.Cats = cats;
            ViewBag.Adoptions = adoptions;
            return View();
        }

        // Add Cat (GET)
        [HttpGet]
        public IActionResult AddCat()
        {
            return View();
        }

        // Add Cat (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCat(Cat cat)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(cat.ImageUrl))
                {
                    cat.ImageUrl = null;  // Set to null if no image URL is provided
                }

                _context.Cats.Add(cat);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cat added successfully.";
                return RedirectToAction("Dashboard");
            }

            return View(cat);
        }

        // Edit Cat (GET)
        [HttpGet]
        public IActionResult EditCat(int id)
        {
            var cat = _context.Cats.Find(id);
            if (cat == null)
                return NotFound();

            return View(cat);
        }

        // Edit Cat (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCat(int id, [Bind("Id,Name,Breed,Age,Description,ImageUrl,AvailableForAdoption")] Cat cat)
        {
            if (id != cat.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingCat = _context.Cats.Find(cat.Id);
                if (existingCat == null)
                {
                    return NotFound();
                }

                // Update cat properties
                existingCat.Name = cat.Name;
                existingCat.Breed = cat.Breed;
                existingCat.Age = cat.Age;
                existingCat.Description = cat.Description;
                existingCat.ImageUrl = string.IsNullOrWhiteSpace(cat.ImageUrl) ? null : cat.ImageUrl;
                existingCat.AvailableForAdoption = cat.AvailableForAdoption;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cat updated successfully.";
                return RedirectToAction("Dashboard");
            }

            return View(cat);
        }

        // Delete Cat (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCat(int id)
        {
            var cat = _context.Cats
                .Include(c => c.Adoptions) // Include related adoptions
                .FirstOrDefault(c => c.Id == id);

            if (cat == null)
            {
                return NotFound(); // Return NotFound if the cat doesn't exist
            }

            // Check if the cat is currently involved in an adoption request
            if (cat.Adoptions.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this cat because it has an active adoption request.";
                return RedirectToAction("Dashboard");
            }

            // Proceed to remove the cat from the database
            _context.Cats.Remove(cat);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cat deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        // View Adoption Details (GET)
        [HttpGet]
        public IActionResult ViewAdoption(int id)
        {
            var adoption = _context.Adoptions
                .Include(a => a.User)
                .Include(a => a.Cat)
                .FirstOrDefault(a => a.Id == id);

            if (adoption == null)
                return NotFound();

            return View(adoption);
        }

        // Accept Adoption (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcceptAdoption(int id)
        {
            var adoption = _context.Adoptions
                .Include(a => a.Cat)
                .FirstOrDefault(a => a.Id == id);

            if (adoption == null)
                return NotFound();

            // Mark the cat as adopted
            adoption.Cat.AvailableForAdoption = false;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Adoption accepted and cat marked as adopted.";
            return RedirectToAction("Dashboard");
        }
        public IActionResult AdopterIndex()
{
    var adopters = _context.Adopter.ToList(); // Assuming you have an Adopter model and DbSet
    return View(adopters); // Pass the list of adopters to the view
}

        // Reject Adoption (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectAdoption(int id)
        {
            var adoption = _context.Adoptions.Find(id);

            if (adoption == null)
                return NotFound();

            _context.Adoptions.Remove(adoption);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Adoption rejected.";
            return RedirectToAction("Dashboard");
        }
    }
}
