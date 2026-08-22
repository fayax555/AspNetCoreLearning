using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class CreateTodoInputModel
    {
        [StringLength(100, ErrorMessage = "Title must be 100 characters or fewer.")]
        [Required(ErrorMessage = "Please provide a title.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please select a priority.")]
        [EnumDataType(typeof(TodoPriority), ErrorMessage = "Please select a valid priority.")]
        public TodoPriority? Priority { get; set; }

        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        public DateOnly? DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int? CategoryId { get; set; }

        public IReadOnlyList<Category> Categories { get; set; } = Array.Empty<Category>();
    }
}
