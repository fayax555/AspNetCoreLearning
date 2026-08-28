using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Models
{
    public class CreateProductInputModel
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(30)]
        public string? Sku { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [Range(0d, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? LastRestockedOn { get; set; }
        public bool IsDiscontinued { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        public IReadOnlyList<Category> Categories { get; set; } = [];
    }
}
