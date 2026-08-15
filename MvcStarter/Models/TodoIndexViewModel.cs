
namespace MvcStarter.Models
{
    public class TodoIndexViewModel
    {
        public required IReadOnlyList<TodoItem> Todos { get; set; }
        public string? Search { get; set; }
    }
}
