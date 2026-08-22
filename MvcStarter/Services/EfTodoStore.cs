using Microsoft.EntityFrameworkCore;
using MvcStarter.Data;
using MvcStarter.Models;

namespace MvcStarter.Services
{
    public class EfTodoStore
    {
        private readonly TodoDbContext _context;

        public EfTodoStore(TodoDbContext context)
        {
            _context = context;
        }

        public TodoItem Add(string title, TodoPriority priority, DateOnly? dueDate, int? categoryId)
        {
            var newTodo = new TodoItem(title, priority);
            newTodo.ChangeCategory(categoryId);
            newTodo.ChangeDueDate(dueDate);
            _context.Todos.Add(newTodo);
            _context.SaveChanges();
            return newTodo;
        }

        public (IReadOnlyList<TodoItem> Todos, int TotalCount)
            GetFilteredTodos(string? search, TodoPriority? priority, int? categoryId, int page, int pageSize)
        {
            var query = _context.Todos.Include(todo => todo.Category).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(todo => EF.Functions.Like(todo.Title, $"%{search!.Trim()}%"));
            }

            if (priority.HasValue)
            {
                query = query.Where(todo => todo.Priority == priority);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(todo => todo.CategoryId == categoryId);
            }

            var totalCount = query.Count();

            var todos = query
                .OrderBy(todo => todo.IsCompleted)
                .ThenBy(todo => todo.Priority)
                .ThenBy(todo => todo.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (todos, totalCount);
        }

        public TodoItem? GetById(int id)
        {
            return _context.Todos
                .Include(todo => todo.Category)
                .AsNoTracking()
                .SingleOrDefault(todo => todo.Id == id);
        }

        public bool TryDelete(int id)
        {
            var todo = _context.Todos.SingleOrDefault(todo => todo.Id == id);
            if (todo == null) return false;
            _context.Remove(todo);
            _context.SaveChanges();

            return true;
        }

        public bool TryMarkCompleted(int id)
        {
            var todo = _context.Todos.SingleOrDefault(todo => todo.Id == id);
            if (todo == null) return false;

            todo.MarkCompleted();
            _context.SaveChanges();
            return true;
        }

        public bool TryUpdate(int id, string title, TodoPriority priority, DateOnly? dueDate, int? categoryId)
        {
            var todo = _context.Todos.SingleOrDefault(todo => todo.Id == id);
            if (todo == null) return false;

            todo.Rename(title);
            todo.ChangePriority(priority);
            todo.ChangeCategory(categoryId);
            todo.ChangeDueDate(dueDate);

            _context.SaveChanges();
            return true;
        }

        public IReadOnlyList<TodoItem> GetByCategoryId(int categoryId)
        {
            return _context.Todos.AsNoTracking()
                .Where(todo => todo.CategoryId == categoryId)
                .OrderBy(todo => todo.IsCompleted)
                .ThenBy(todo => todo.Priority)
                .ThenBy(todo => todo.Id)
                .ToList();
        }
    }
}
