namespace InventoryManager.Models
{
    public class ProductIndexViewModel
    {
        public IReadOnlyList<Product> Products { get; set; } = [];
        public string? Search { get; set; }
    }
}
