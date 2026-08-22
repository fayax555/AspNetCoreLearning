using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class EditCategoryInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a category name.")]
        [StringLength(50, ErrorMessage = "Category name must be 50 characters or fewer.")]
        public string? Name { get; set; }
    }
}
