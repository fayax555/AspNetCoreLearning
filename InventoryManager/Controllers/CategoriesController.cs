using InventoryManager.Data;
using InventoryManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManager.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly InventoryDbContext _context;

        public CategoriesController(InventoryDbContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.OrderBy(category => category.Name).ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CategoryInputModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                ModelState.AddModelError(nameof(input.Name), "Please provide category a name");
            }

            if (!ModelState.IsValid) return View(input);

            var trimmedName = input.Name!.Trim();

            var categoryExists = _context.Categories.Any(category =>
                category.Name.ToUpper() == trimmedName.ToUpper());

            if (categoryExists)
            {
                ModelState.AddModelError(nameof(input.Name), "The provided category name already exists");
            }

            if (!ModelState.IsValid) return View(input);

            var category = new Category { Name = trimmedName };
            _context.Categories.Add(category);
            _context.SaveChanges();

            TempData["Success"] = "Category has been added";

            return RedirectToAction(nameof(Index));
        }
    }
}
