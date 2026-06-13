using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace shop.api.Data.Models
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var db = services.GetRequiredService<DB>();

            // ── Тестовые пользователи ──
            if (!await userManager.Users.AnyAsync())
            {
                var buyer = new User
                {
                    Email = "buyer@test.com",
                    UserName = "buyer@test.com",
                    FullName = "Иван Покупатель",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(buyer, "Test123!");
                await userManager.AddToRoleAsync(buyer, "Buyer");

                var seller = new User
                {
                    Email = "seller@test.com",
                    UserName = "seller@test.com",
                    FullName = "Анна Мастер",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(seller, "Test123!");
                await userManager.AddToRoleAsync(seller, "Seller");

                var admin = new User
                {
                    Email = "admin@test.com",
                    UserName = "admin@test.com",
                    FullName = "Администратор",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Test123!");
                await userManager.AddToRoleAsync(admin, "Admin");

                // ── Тестовые товары для seller ──
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Вязаный свитер из мериноса",
                    Price = 2400,
                    Quantity = 3,
                    IsAvailable = true,
                    Info = new ProductInfo
                    {
                        Id = Guid.NewGuid(),
                        FullDescription = "Тёплый свитер ручной вязки из 100% мериносовой шерсти.",
                        ShortDescription = "Вязаный свитер, тёплый и мягкий",
                        Category = "Вязание",
                        Material = "Шерсть",
                        Color = "Синий",
                        Width = 50,
                        Height = 70
                    }
                };
                product.Info.ProductId = product.Id;

                db.Products.Add(product);
                await db.SaveChangesAsync();
            }
        }
    }
}
