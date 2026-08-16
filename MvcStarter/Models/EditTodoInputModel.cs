using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class EditTodoInputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please select a priority.")]
        [EnumDataType(typeof(TodoPriority), ErrorMessage = "Please select a valid priority.")]
        public TodoPriority? Priority { get; set; }
    }
}
