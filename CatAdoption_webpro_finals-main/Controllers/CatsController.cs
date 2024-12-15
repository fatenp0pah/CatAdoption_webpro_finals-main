using Microsoft.AspNetCore.Mvc;
using CatAdoption.Models;
using CatAdoption.Data;
using System.Linq;

namespace CatAdoption.Controllers
{
    public class CatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cats
        public IActionResult Index()
        {
            return View(_context.Cats.ToList());
        }

        // GET: Cats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cats/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // For CSRF protection
        public IActionResult Create([Bind("Name,Breed,Age,Description,ImageUrl,AvailableForAdoption")] Cat cat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cat);
                _context.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
            }
            return View(cat); // If validation fails, return the same view with the model data
        }
        

        // Edit and Delete actions can go here as needed
    }
}
