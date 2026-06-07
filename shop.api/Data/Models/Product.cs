namespace shop.api.Data.Models
{
    public class Product
    {

        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public User Seller { get; set; } = null!;
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsAvailable { get; set; } = true;

        public ProductInfo? Info { get; set; }
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    }
}
