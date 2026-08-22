namespace MvcStarter.Models
{
    public class CategoryDetailsViewModel
    {
        public required Category Category { get; set; }
        public required IReadOnlyList<TodoItem> Todos { get; set; }
    }
}
