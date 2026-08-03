using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shop.api.Data.Models;
using System.Security.Claims;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DB db;
    public OrdersController(DB db) { this.db = db; }

        [Authorize]
        [HttpPost("api/orders")]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var buyerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = dto.Items.Sum(x => x.UnitPrice * x.Quantity),
                Items = dto.Items.Select(x => new OrderItems
                {
                    Id = Guid.NewGuid(),
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList()
            };

            this.db.Orders.Add(order);
            await this.db.SaveChangesAsync();
            return Ok(new { order.Id });
        }

        [Authorize]
        [HttpDelete("api/orders/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var order = await this.db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == userId);
            if (order == null) return NotFound();
            this.db.Orders.Remove(order);
            await this.db.SaveChangesAsync();
            return Ok();
        }






        [Authorize]
        [HttpGet("api/orders/my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var orders = await this.db.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.Id,
                o.Status,
                o.TotalPrice,
                o.CreatedAt,
                Items = o.Items.Select(i => new
                {
                    Title = i.Product.Title,
                    i.Quantity,
                    i.UnitPrice,
                   ImageUrl = i.Product.Images.FirstOrDefault()?.ImageUrl
                }).ToList()
            }));
        }

        public record CreateOrderDto(List<OrderItemDto> Items);
        public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
   
    }
}
