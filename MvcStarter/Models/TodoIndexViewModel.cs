
namespace MvcStarter.Models
{
    public class TodoIndexViewModel
    {
        public required IReadOnlyList<TodoItem> Todos { get; set; }
        public string? Search { get; set; }
        public TodoPriority? SelectedPriority { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? SelectedCategoryId { get; set; }
        public IReadOnlyList<Category> Categories { get; set; } = Array.Empty<Category>();
        public bool OverdueOnly { get; set; }
    }
}
