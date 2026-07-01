namespace ProductService.API.Models
{
    // Product Table
    public class Product
    {
        public int ProductId { get; set; }                 // Primary Key
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }                  // Available Quantity (stock)
        
        // Navigation property (One Product -> Many InvoiceItems)
        //public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
