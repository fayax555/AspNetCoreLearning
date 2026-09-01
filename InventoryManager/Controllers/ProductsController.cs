using InventoryManager.Data;
using InventoryManager.Models;
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

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.OrderBy(category => category.Name).ToList();
            var model = new CreateProductInputModel { Categories = categories };
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CreateProductInputModel input)
        {
            var categoryExists = _context.Categories
                .Any((category) => category.Id == input.CategoryId);

            if (input.CategoryId.HasValue && !categoryExists)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _context.Categories.OrderBy(category => category.Name).ToList();
                return View(input);
            }

            var trimmedSku = input.Sku!.Trim();
            var skuExists = _context.Products
                .Any(product => product.Sku.ToUpper() == trimmedSku.ToUpper());

            if (skuExists)
            {
                ModelState.AddModelError(nameof(input.Sku), "Sku already used by another product");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _context.Categories.OrderBy(category => category.Name).ToList();
                return View(input);
            }

            var product = new Product
            {
                Name = input.Name!.Trim(),
                Sku = trimmedSku,
                Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
                QuantityInStock = input.QuantityInStock,
                ReorderLevel = input.ReorderLevel,
                UnitPrice = input.UnitPrice,
                LastRestockedOn = input.LastRestockedOn,
                IsDiscontinued = input.IsDiscontinued,
                CategoryId = input.CategoryId!.Value,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            TempData["Success"] = "Product added";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.SingleOrDefault(product => product.Id == id);
            if (product == null) return NotFound();

            var categories = _context.Categories.OrderBy(category => category.Name).ToList();

            var model = new EditProductInputModel
            {
                Id = id,
                Name = product.Name!.Trim(),
                Sku = product.Sku,
                Description = product.Description,
                QuantityInStock = product.QuantityInStock,
                ReorderLevel = product.ReorderLevel,
                UnitPrice = product.UnitPrice,
                LastRestockedOn = product.LastRestockedOn,
                IsDiscontinued = product.IsDiscontinued,
                CategoryId = product.CategoryId,
                Categories = categories,
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(EditProductInputModel input)
        {
            var product = _context.Products.SingleOrDefault(product => product.Id == input.Id);
            if (product == null) return NotFound();

            var categoryExists = _context.Categories
                .Any((category) => category.Id == input.CategoryId);

            if (input.CategoryId.HasValue && !categoryExists)
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _context.Categories.OrderBy(category => category.Name).ToList();
                return View(input);
            }

            var trimmedSku = input.Sku!.Trim();
            var skuExists = _context.Products
                .Any(product => product.Sku.ToUpper() == trimmedSku.ToUpper() && product.Id != input.Id);

            if (skuExists)
            {
                ModelState.AddModelError(nameof(input.Sku), "Sku already used by another product");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _context.Categories.OrderBy(category => category.Name).ToList();
                return View(input);
            }

            product.Name = input.Name!.Trim();
            product.Sku = trimmedSku;
            product.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
            product.QuantityInStock = input.QuantityInStock;
            product.ReorderLevel = input.ReorderLevel;
            product.UnitPrice = input.UnitPrice;
            product.LastRestockedOn = input.LastRestockedOn;
            product.IsDiscontinued = input.IsDiscontinued;
            product.CategoryId = input.CategoryId!.Value;

            _context.SaveChanges();

            TempData["Success"] = "Product Updated";

            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.SingleOrDefault(product => product.Id == id);

            if (product == null) return NotFound();

            _context.Remove(product);
            _context.SaveChanges();

            TempData["Success"] = "Product deleted";

            return RedirectToAction(nameof(Index));
        }
    }
}
