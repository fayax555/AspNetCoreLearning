using Microsoft.AspNetCore.Mvc;
using MvcStarter.Models;
using MvcStarter.Services;

namespace MvcStarter.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly EfTodoStore _todoStore;

        public CategoriesController(EfTodoStore todoStore)
        {
            _todoStore = todoStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _todoStore.GetCategories();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateCategoryInputModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel input)
        {
            if (ModelState.IsValid && _todoStore.CategoryNameExists(input.Name!))
            {
                ModelState.AddModelError(nameof(input.Name), "A category with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            _todoStore.AddCategory(input.Name!);
            TempData["Success"] = "Category added.";

            return RedirectToAction(nameof(Index));
        }
    }
}
