using MvcStarter.Models;

namespace MvcStarter.Services
{
    public interface ITodoStore
    {
        IReadOnlyList<TodoItem> GetAll();

        TodoItem Add(string title);

        bool TryMarkCompleted(int id);

        bool TryDelete(int id);

        TodoItem? GetById(int id);

        bool TryRename(int id, string title);
    }
}
