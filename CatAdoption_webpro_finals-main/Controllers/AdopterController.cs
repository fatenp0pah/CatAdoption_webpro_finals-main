using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatAdoption.Data;
using CatAdoption.Models;
using System.Threading.Tasks;

public class AdopterController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdopterController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Adopter/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Adopter/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Adopter adopter)
    {
        if (ModelState.IsValid)
        {
            // Add the adopter to the database
            _context.Adopter.Add(adopter);
            await _context.SaveChangesAsync();

            // Redirect to the list of adopters after creation
            return RedirectToAction(nameof(Index));
        }

        // Return the view with the model if the data is invalid
        return View(adopter);
    }

    // GET: Adopter/Index
    public async Task<IActionResult> Index()
    {
        var adopter = await _context.Adopter.ToListAsync();
        return View(adopter);
    }
    // Edit Adopter (GET)
[HttpGet]
public IActionResult EditAdopter(int id)
{
    var adopter = _context.Adopter.Find(id);
    if (adopter == null)
        return NotFound();

    return View(adopter);
}
// Edit Adopter (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult EditAdopter(int id, [Bind("Id,username,catname")] Adopter adopter)
{
    if (id != adopter.Id)
        return NotFound();

    if (ModelState.IsValid)
    {
        var existingAdopter = _context.Adopter.Find(adopter.Id);
        if (existingAdopter == null)
        {
            return NotFound();
        }

        // Update adopter properties
        existingAdopter.username = adopter.username;
        existingAdopter.catname = adopter.catname;

        _context.SaveChanges();
        TempData["SuccessMessage"] = "Adopter updated successfully.";
        return RedirectToAction("Index"); // Redirect to the list of adopters
    }

    return View(adopter); // Return the view if the model is invalid
}
// Delete Adopter (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult DeleteAdopter(int id)
{
    var adopter = _context.Adopter.Find(id);
    if (adopter == null)
    {
        return NotFound(); // Return NotFound if the adopter doesn't exist
    }

    _context.Adopter.Remove(adopter); // Remove adopter from the database
    _context.SaveChanges();

    TempData["SuccessMessage"] = "Adopter deleted successfully."; // Success message
    return RedirectToAction("Index"); // Redirect to the list of adopters
}


}
