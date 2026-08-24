using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class TodoFilterInputModel
    {
        [StringLength(100, ErrorMessage = "Search must be 100 characters or fewer.")]
        public string? Search { get; set; }

        [EnumDataType(typeof(TodoPriority), ErrorMessage = "Please select a valid priority.")]
        public TodoPriority? SelectedPriority { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int? SelectedCategoryId { get; set; }

        public bool OverdueOnly { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page must be 1 or greater.")]
        public int Page { get; set; } = 1;
    }
}
