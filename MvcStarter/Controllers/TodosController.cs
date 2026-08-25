using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcStarter.Models;
using MvcStarter.Services;
using MvcStarter.Settings;

namespace MvcStarter.Controllers
{
    public class TodosController : Controller
    {
        private readonly EfTodoStore _todoStore;
        private readonly EfCategoryStore _categoryStore;
        private readonly ILogger<TodosController> _logger;
        private readonly TodoSettings _todoSettings;

        public TodosController(
            EfTodoStore todoStore,
            EfCategoryStore categoryStore,
            ILogger<TodosController> logger,
            IOptions<TodoSettings> todoSettingsOptions)
        {
            _todoStore = todoStore;
            _categoryStore = categoryStore;
            _logger = logger;
            _todoSettings = todoSettingsOptions.Value;
        }

        [HttpGet]
        public IActionResult Index(TodoFilterInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pageSize = _todoSettings.PageSize;

            var currentDate = DateOnly.FromDateTime(DateTime.Today);
            var (pageTodos, totalCount) = _todoStore.GetFilteredTodos(
                input.Search,
                input.SelectedPriority,
                input.SelectedCategoryId,
                input.OverdueOnly,
                currentDate,
                input.Page,
                pageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages > 0 && input.Page > totalPages)
            {
                return NotFound();
            }

            var model = new TodoIndexViewModel
            {
                Todos = pageTodos,
                Search = input.Search,
                SelectedPriority = input.SelectedPriority,
                CurrentPage = input.Page,
                TotalPages = totalPages,
                SelectedCategoryId = input.SelectedCategoryId,
                OverdueOnly = input.OverdueOnly,
                Categories = _categoryStore.GetCategories(),
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateTodoInputModel
            {
                Categories = _categoryStore.GetCategories()
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CreateTodoInputModel input)
        {
            if (input.CategoryId.HasValue && !_categoryStore.CategoryExists(input.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _categoryStore.GetCategories();
                return View(input);
            }

            var todo = _todoStore.Add(input.Title!, input.Priority!.Value, input.DueDate, input.CategoryId);
            _logger.LogInformation("Created todo {TodoId} with title {TodoTitle}", todo.Id, todo.Title);

            TempData["SuccessMessage"] = "Todo Created.";
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Complete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_todoStore.TryMarkCompleted(id))
            {
                _logger.LogWarning("Could not complete todo {TodoId} because it was not found", id);
                return NotFound();
            }

            _logger.LogInformation("Completed todo {TodoId}", id);

            TempData["SuccessMessage"] = "Todo Completed.";

            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_todoStore.TryDelete(id))
            {
                _logger.LogWarning("Could not delete todo {TodoId} because it was not found", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted todo {TodoId}", id);

            TempData["SuccessMessage"] = "Todo Deleted.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = _todoStore.GetById(id);

            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todoInDb = _todoStore.GetById(id);

            if (todoInDb == null)
            {
                return NotFound();
            }

            var model = new EditTodoInputModel
            {
                Id = todoInDb.Id,
                Title = todoInDb.Title,
                Priority = todoInDb.Priority,
                DueDate = todoInDb.DueDate,
                CategoryId = todoInDb.CategoryId,
                Categories = _categoryStore.GetCategories(),
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(EditTodoInputModel input)
        {
            if (input.CategoryId.HasValue && !_categoryStore.CategoryExists(input.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _categoryStore.GetCategories();
                return View(input);
            }

            if (!_todoStore.TryUpdate(input.Id, input.Title!, input.Priority!.Value, input.DueDate, input.CategoryId))
            {
                _logger.LogWarning("Could not edit todo {TodoId} because it was not found", input.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated todo {TodoId} with title {TodoTitle}", input.Id, input.Title);

            TempData["SuccessMessage"] = "Todo Edited.";

            return RedirectToAction(nameof(Index));
        }
    }
}
