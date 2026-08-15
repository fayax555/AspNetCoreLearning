using Microsoft.EntityFrameworkCore;
using MvcStarter.Data;
using MvcStarter.Models;

namespace MvcStarter.Services
{
    public class EfTodoStore : ITodoStore
    {
        private readonly TodoDbContext _context;

        public EfTodoStore(TodoDbContext context)
        {
            _context = context;
        }

        public TodoItem Add(string title)
        {
            var newTodo = new TodoItem(title);
            _context.Todos.Add(newTodo);
            _context.SaveChanges();
            return newTodo;
        }

        public IReadOnlyList<TodoItem> GetAll()
        {
            return _context.Todos.AsNoTracking().ToList();
        }

        public TodoItem? GetById(int id)
        {
            return _context.Todos.AsNoTracking().SingleOrDefault(todo => todo.Id == id);
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

        public bool TryRename(int id, string title)
        {
            var todo = _context.Todos.SingleOrDefault(todo => todo.Id == id);
            if (todo == null) return false;

            todo.Rename(title);
            _context.SaveChanges();
            return true;
        }
    }
}
