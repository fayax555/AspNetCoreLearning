using MvcStarter.Models;

namespace MvcStarter.Services
{
    public class TodoStore : ITodoStore
    {
        private readonly object _sync = new object();
        private int _nextId = 4;

        private readonly List<TodoItem> _todos = new List<TodoItem>
            {
                new TodoItem(1, "Learn C#", true),
                new TodoItem(2, "Build MVC app", false),
                new TodoItem(3, "Go for a walk", false),
            };

        public IReadOnlyList<TodoItem> GetAll()
        {
            lock (_sync)
            {
                return _todos.ToList();
            }
        }

        public TodoItem Add(string title)
        {
            lock (_sync)
            {
                var todo = new TodoItem(_nextId, title, false);
                _todos.Add(todo);
                _nextId++;
                return todo;
            }
        }

        public bool TryMarkCompleted(int id)
        {
            lock (_sync)
            {
                var todo = _todos.SingleOrDefault(todo => todo.Id == id);
                if (todo == null) return false;

                todo.MarkCompleted();
                return true;
            }
        }

        public bool TryDelete(int id)
        {
            lock (_sync)
            {
                var todo = _todos.SingleOrDefault(todo => todo.Id == id);
                if (todo == null) return false;

                _todos.Remove(todo);
                return true;
            }
        }

        public TodoItem? GetById(int id)
        {
            lock (_sync)
            {
                return _todos.SingleOrDefault(todo => todo.Id == id);
            }
        }

        public bool TryRename(int id, string title)
        {
            lock (_sync)
            {
                var todo = _todos.SingleOrDefault(todo => todo.Id == id);
                if (todo == null) return false;

                todo.Rename(title);
                return true;
            }
        }
    }
}
