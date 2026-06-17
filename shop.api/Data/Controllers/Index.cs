using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace shop.api.Data.Controllers
{
    public class Index : Controller
    {
        private readonly DB db;

        public Index(DB db)
        {
            this.db = db;
        }

        [HttpGet("api/products")]
        public async Task<IActionResult> GetAll([FromQuery] string? category)
        {
            var query = db.Products
                .Include(p => p.Info)
                .Include(p => p.Images)
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Info != null && p.Info.Category == category);

            var result = await query
                .OrderByDescending(p => p.Id)
                .Select(p => new ProductCardDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Category = p.Info != null ? p.Info.Category : "",
                    ImageUrl = p.Images.FirstOrDefault()!.ImageUrl,
                    SellerId = p.SellerId
                })
                .ToListAsync();

            return Ok(result);
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


    }
}
