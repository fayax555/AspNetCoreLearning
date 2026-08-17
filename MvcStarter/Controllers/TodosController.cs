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
        public IActionResult Index(string? search, TodoPriority? selectedPriority, int page = 1)
        {
            if (page < 1)
            {
                return BadRequest();
            }

            var filteredTodos = _todoStore.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
               filteredTodos = filteredTodos
                    .Where(todo => todo.Title.Contains(search!.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (selectedPriority.HasValue)
            {
                filteredTodos = filteredTodos
                    .Where(todo => todo.Priority == selectedPriority.Value)
                    .ToList();
            }

            filteredTodos = filteredTodos
                .OrderBy(todo => todo.IsCompleted)
                .ThenBy(todo => todo.Priority)
                .ThenBy(todo => todo.Id)
                .ToList();

            const int pageSize = 3;

            var totalPages = (int)Math.Ceiling(filteredTodos.Count / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                return NotFound();
            }

            var pageTodos = filteredTodos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new TodoIndexViewModel
            {
                Todos = pageTodos,
                Search = search,
                SelectedPriority = selectedPriority,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateTodoInputModel();

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CreateTodoInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            _todoStore.Add(input.Title!, input.Priority!.Value);

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
                Priority = todoInDb.Priority
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(EditTodoInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            if (!_todoStore.TryUpdate(input.Id, input.Title!, input.Priority!.Value))
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Todo Edited.";

            return RedirectToAction(nameof(Index));
        }
    }
}
