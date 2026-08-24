namespace MvcStarter.Models
{
    public class TodoFilterInputModel
    {
        public string? Search { get; set; }
        public TodoPriority? SelectedPriority { get; set; }
        public int? SelectedCategoryId { get; set; }
        public bool OverdueOnly { get; set; }
        public int Page { get; set; } = 1;
    }
}
