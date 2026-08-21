using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class CreateCategoryInputModel
    {
        [Required(ErrorMessage = "Please provide a category name.")]
        [StringLength(50, ErrorMessage = "Category names must be 50 characters or fewer.")]
        public string? Name { get; set; }
    }
}
