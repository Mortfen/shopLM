using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shop.api.Data.Models;
using System;
using System.Security.Claims;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DB db;

        public ProductsController(DB db)
        {
            this.db = db;
        }
        //фото
        [Authorize(Roles = "Seller")]
        [HttpPost("api/products/{id}/images")]
        public async Task<IActionResult> UploadImages(Guid id, [FromForm] List<IFormFile> files)
        {

            var product = await db.Products.FindAsync(id);
            if (product == null) { return NotFound(); }

            var uploadsPath = Path.Combine("wwwroot", "uploads", "products");
            Directory.CreateDirectory(uploadsPath); // создаст папку если её нет

            foreach (var file in files)
            {
                var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                db.ProductImages.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    ImageUrl = $"/uploads/products/{fileName}"
                });
            }
            await db.SaveChangesAsync();
            return Ok();
        }


        //info
        [HttpGet("api/products/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await this.db.Products
                .Include(p => p.Info)
                .Include(p => p.Images)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return Ok(new
            {
                product.Id,
                product.Title,
                product.Price,
                product.Quantity,
                product.IsAvailable,
                SellerName = product.Seller.FullName,
                Info = product.Info == null ? null : new
                {
                    product.Info.FullDescription,
                    product.Info.Category,
                    product.Info.Material,
                    product.Info.Color,
                    product.Info.Width,
                    product.Info.Height
                },
                Images = product.Images.Select(i => i.ImageUrl).ToList()
            });
        }


        //сощдание
        [Authorize(Roles = "Seller")]
        [HttpPost("api/products")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var sellerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                SellerId = sellerId,
                Title = dto.Name,
                Price = dto.Price,
                Quantity = dto.Quantity,
                IsAvailable = true,
                Info = new ProductInfo
                {
                    Id = Guid.NewGuid(),
                    FullDescription = dto.Description,
                    ShortDescription = dto.Description.Length > 120
                        ? dto.Description[..120]
                        : dto.Description,
                    Category = dto.Category,
                    Material = dto.Material,
                    Color = dto.Color,
                    Width = dto.Width,
                    Height = dto.Height
                }
            };
            product.Info.ProductId = product.Id;

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return Ok(new { product.Id });
        }
        public class CreateProductDto
        {
            public string Name { get; set; } = "";
            public string Category { get; set; } = "";
            public string Description { get; set; } = "";
            public string Color { get; set; } = "";
            public string Material { get; set; } = "";
            public int Width { get; set; }
            public int Height { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; } = 1;
        }
     }
}


