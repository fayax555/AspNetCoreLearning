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

        public TodoItem Add(string title, TodoPriority priority, int? categoryId)
        {
            var newTodo = new TodoItem(title, priority);
            newTodo.ChangeCategory(categoryId);
            _context.Todos.Add(newTodo);
            _context.SaveChanges();
            return newTodo;
        }

        public IReadOnlyList<TodoItem> GetAll()
        {
            return _context.Todos.Include(todo => todo.Category).AsNoTracking().ToList();
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

        public bool TryUpdate(int id, string title, TodoPriority priority, int? categoryId)
        {
            var todo = _context.Todos.SingleOrDefault(todo => todo.Id == id);
            if (todo == null) return false;

            todo.Rename(title);
            todo.ChangePriority(priority);
            todo.ChangeCategory(categoryId);

            _context.SaveChanges();
            return true;
        }

        public IReadOnlyList<Category> GetCategories()
        {
            return _context.Categories.AsNoTracking().OrderBy(category => category.Name).ToList();
        }


    }
}
