using Microsoft.AspNetCore.Mvc;
using CatAdoption.Data;
using CatAdoption.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CatAdoption.Controllers
{
    public class AdoptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdoptionsController> _logger;

        public AdoptionsController(ApplicationDbContext context, ILogger<AdoptionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // View adoption requests with related Cat and User data
        public async Task<IActionResult> Approval()
        {
            var adoptions = await _context.Adoptions
                .Include(a => a.Cat)
                .Include(a => a.User)
                .ToListAsync();

            return View(adoptions);
        }

        // GET: Adoption/Create
        public ActionResult Create()
        {
            // Filter only available cats for adoption
            ViewBag.Cats = _context.Cats.Where(c => c.AvailableForAdoption).ToList();
            ViewBag.Users = _context.Users.ToList(); // Get all users for dropdown
            return View();
        }

        // POST: Adoption/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adoption adoption)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Automatically set the CreatedAt field to the current date and time
                    adoption.CreatedAt = DateTime.UtcNow;

                    // Add adoption to the database
                    _context.Adoptions.Add(adoption);

                    // Update the cat's availability
                    var cat = await _context.Cats.FindAsync(adoption.CatId);
                    if (cat != null && cat.AvailableForAdoption)
                    {
                        cat.AvailableForAdoption = false;
                        _context.Cats.Update(cat);
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Dashboard", "Admin"); // Redirect to the dashboard
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the saving process
                    _logger.LogError($"Error while saving adoption: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the adoption.");
                }
            }

            // Reload filtered data for dropdown lists if there is an error
            ViewBag.Cats = _context.Cats.Where(c => c.AvailableForAdoption).ToList();
            ViewBag.Users = _context.Users.ToList();
            return View(adoption);
        }
    }

    public class AdoptionRequestViewModel
    {
        public int Id { get; set; }
        public required string CatName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
