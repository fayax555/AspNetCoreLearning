namespace InventoryManager.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public decimal UnitPrice { get; set; }
        public DateOnly? LastRestockedOn { get; set; }
        public bool IsDiscontinued { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
