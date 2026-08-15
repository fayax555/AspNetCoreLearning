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
    }
}
