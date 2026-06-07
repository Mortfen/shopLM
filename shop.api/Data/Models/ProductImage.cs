namespace shop.api.Data.Models
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ImageUrl { get; set; } = "";
    }
}
