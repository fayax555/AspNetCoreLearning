using Microsoft.AspNetCore.Mvc;
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
    }
}
