using InventoryManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Controllers
{
    public class ProductsController : Controller
    {
        private readonly InventoryDbContext _context;

        public ProductsController(InventoryDbContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(product => product.Category)
                .OrderBy(product => product.Name)
                .ToList();
            return View(products);
        }
    }
}
