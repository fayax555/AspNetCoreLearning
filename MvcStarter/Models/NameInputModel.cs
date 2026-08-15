using System.ComponentModel.DataAnnotations;

namespace MvcStarter.Models
{
    public class NameInputModel
    {
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 30 characters.")]
        [Required(ErrorMessage = "Please provide a name.")]
        public string? Name { get; set; }
    }
}
