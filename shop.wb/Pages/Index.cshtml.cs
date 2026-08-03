using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;

namespace shop.wb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly API aPI;

        private readonly IConfiguration config;

        public IndexModel(API aPI, IConfiguration config)
        {
            this.aPI = aPI;
            this.config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }
        public int SellerCount => Product.Select(p => p.SellerId).Distinct().Count();

        public string ApiBase => this.config["ApiBaseUrl"] ?? "http://hobbyshop-api:8080";
        // public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public List<ProductCardDto> Product { get; set; } = new List<ProductCardDto>();
        public async Task OnGetAsync() 
        {
            var url = string.IsNullOrEmpty(Category)
              ? "api/products"
             : $"api/products?category={Uri.EscapeDataString(Category)}";
            Product = await aPI.GetAsync<List<ProductCardDto>>(url) ?? new();

        }

        [Authorize]
        public async Task<IActionResult> OnPostBuy(Guid productId, decimal price)
        {
            var dto = new
            {
                Items = new[]
                {
            new
            {
                ProductId = productId,
                Quantity = 1,
                UnitPrice = price
            }
        }
            };

            await aPI.PostAsync("api/orders", dto);

            return RedirectToPage("/Account/Orders");
        }



    }
    public class ProductCardDto 
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string? ImageUrl { get; set; }
        public Guid SellerId { get; set; }
    }
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
    public class OrderItemDto
    {
        public string Title { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //   public string? ImageUrl { get; set; }
    }
}
