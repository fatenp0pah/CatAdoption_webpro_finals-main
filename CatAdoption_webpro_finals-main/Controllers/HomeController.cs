using Microsoft.AspNetCore.Mvc;
using CatAdoption.Models;
using CatAdoption.Data; // Add this for access to ApplicationDbContext
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Diagnostics; // For LINQ queries

namespace CatAdoption.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Injecting the ApplicationDbContext

        // Constructor to inject logger and ApplicationDbContext
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Assign the ApplicationDbContext to _context
        }

        // Index action to display the list of cats
        public IActionResult Index()
        {
            // Fetch all cats from the database (including Age and AvailableForAdoption)
            var cats = _context.Cats.ToList(); // Ensure this includes Age and AvailableForAdoption

            // Pass the list of cats to the view
            return View(cats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
