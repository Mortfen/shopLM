namespace shop.api.Data.Models
{
    public class ProductInfo
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string FullDescription { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string Category { get; set; } = "";
        public string Material { get; set; } = "";
        public string Color { get; set; } = "";
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Length { get; set; }
        public int? Weight { get; set; }

        public Product Product { get; set; } = null!;
    }
}
