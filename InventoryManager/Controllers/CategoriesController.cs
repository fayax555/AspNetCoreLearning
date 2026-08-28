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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null) return NotFound();

            var model = new EditCategoryInputModel { Id = id, Name = category.Name };
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(EditCategoryInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var trimmedName = input.Name!.Trim();
            var categoryExists = _context.Categories.Any(category =>
                category.Name.ToUpper() == trimmedName.ToUpper() && category.Id != input.Id);

            if (categoryExists)
            {
                ModelState.AddModelError(nameof(input.Name), "Category name already exists");
            }

            if (!ModelState.IsValid) return View(input);

            var category = _context.Categories.SingleOrDefault(category => category.Id == input.Id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = trimmedName;

            _context.SaveChanges();

            TempData["Success"] = "Category has been updated";

            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);
            if (category == null) return NotFound();

            var categoryHasProducts = _context.Products.Any(product => product.CategoryId == id);
            if (categoryHasProducts)
            {
                TempData["Error"] = "This category cannot be deleted because it contains products.";
                return RedirectToAction(nameof(Index));
            }

            _context.Remove(category);
            _context.SaveChanges();

            TempData["Success"] = "Category has been deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
