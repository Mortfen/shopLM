using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;
using System.Text.Json;

namespace shop.wb.Pages.Account
{
   [Authorize]
    public class OrdersModel : PageModel
    {
        private readonly API api;

        private readonly IConfiguration config;

        public List<OrdersDto> Orders { get; set; } = new List<OrdersDto>();

        public OrdersModel(API api, IConfiguration config)
        {
            this.api = api;
            this.config = config;
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await this.api.DeleteAsync($"api/orders/{id}");
            return RedirectToPage();
        }



        public string ApiBase => this.config["ApiBaseUrl"] ?? "https://localhost:7168";
        public async Task OnGetAsync()
        {
            Orders = await api.GetAsync<List<OrdersDto>>("api/orders/my")
                     ?? new();
        }
        public int TotalCount =>
    Orders.SelectMany(o => o.Items).Sum(i => i.Quantity);

        public decimal TotalPrice =>
            Orders.SelectMany(o => o.Items).Sum(i => i.UnitPrice * i.Quantity);

    }
    public class OrdersDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public string? ImageUrl { get; set; }
    }
    public class OrderItemDto
    {
        public string Title { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
