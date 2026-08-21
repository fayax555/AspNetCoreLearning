using Microsoft.AspNetCore.Mvc;
using MvcStarter.Models;
using MvcStarter.Services;

namespace MvcStarter.Controllers
{
    public class TodosController : Controller
    {
        private readonly EfTodoStore _todoStore;

        public TodosController(EfTodoStore todoStore)
        {
            _todoStore = todoStore;
        }

        [HttpGet]
        public IActionResult Index(string? search, TodoPriority? selectedPriority, int? selectedCategoryId, int page = 1)
        {
            if (page < 1)
            {
                return BadRequest();
            }

            const int pageSize = 3;

            var (pageTodos, totalCount) = _todoStore.GetFilteredTodos(search, selectedPriority, selectedCategoryId, page, pageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                return NotFound();
            }

            var model = new TodoIndexViewModel
            {
                Todos = pageTodos,
                Search = search,
                SelectedPriority = selectedPriority,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedCategoryId = selectedCategoryId,
                Categories = _todoStore.GetCategories(),
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateTodoInputModel
            {
                Categories = _todoStore.GetCategories()
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CreateTodoInputModel input)
        {
            if (input.CategoryId.HasValue && !_todoStore.CategoryExists(input.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _todoStore.GetCategories();
                return View(input);
            }

            _todoStore.Add(input.Title!, input.Priority!.Value, input.CategoryId);

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
                return NotFound();
            }

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
                return NotFound();
            }

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
                CategoryId = todoInDb.CategoryId,
                Categories = _todoStore.GetCategories(),
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(EditTodoInputModel input)
        {
            if (input.CategoryId.HasValue && !_todoStore.CategoryExists(input.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(input.CategoryId), "Please select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                input.Categories = _todoStore.GetCategories();
                return View(input);
            }

            if (!_todoStore.TryUpdate(input.Id, input.Title!, input.Priority!.Value, input.CategoryId))
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Todo Edited.";

            return RedirectToAction(nameof(Index));
        }
    }
}
