using InventoryManager.Data;
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
    }
}
