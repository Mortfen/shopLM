using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;

namespace shop.wb.Pages.Producs
{
    public class DetailsModel : PageModel
    {
        private readonly API api;
    private readonly IConfiguration config;

    public ProductDto? Product { get; set; }
        public string ApiBase => this.config["ApiBaseUrl"] ?? "https://localhost:7168";

        public DetailsModel(API api, IConfiguration config)
        {
            this.api = api;
            this.config = config;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Product = await this.api.GetAsync<ProductDto>($"api/products/{id}");
            if (Product == null) return NotFound();
            return Page();
        }

        public class ProductDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = "";
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public bool IsAvailable { get; set; }
            public string SellerName { get; set; } = "";
            public ProductInfoDto? Info { get; set; }
            public List<string> Images { get; set; } = new();
        }

        public class ProductInfoDto
        {
            public string? FullDescription { get; set; }
            public string? Category { get; set; }
            public string? Material { get; set; }
            public string? Color { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
