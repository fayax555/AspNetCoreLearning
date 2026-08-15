using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class EditTodoInputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }
    }
}
