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

            // 1. Пользователи (добавляются только если БД пустая)
            if (!await userManager.Users.AnyAsync())
            {
                var buyer = new User { Email = "buyer@test.com", UserName = "buyer@test.com", FullName = "Иван Покупатель", EmailConfirmed = true };
                await userManager.CreateAsync(buyer, "Test123!");
                await userManager.AddToRoleAsync(buyer, "Buyer");

                // Здесь переменная называется newSeller
                var newSeller = new User { Email = "seller@test.com", UserName = "seller@test.com", FullName = "Анна Мастер", EmailConfirmed = true };
                await userManager.CreateAsync(newSeller, "Test123!");
                await userManager.AddToRoleAsync(newSeller, "Seller");

                var admin = new User { Email = "admin@test.com", UserName = "admin@test.com", FullName = "Администратор", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Test123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 2. Добавляем товары
            // Здесь переменную назвали currentSeller, чтобы не было конфликта имён
            var currentSeller = await userManager.FindByEmailAsync("seller@test.com");

            if (currentSeller != null)
            {
                var productsToSeed = new List<Product>
        {
            // Обратите внимание: везде используем currentSeller.Id
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Вязаный свитер из мериноса", Price = 2400, Quantity = 3, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Тёплый свитер ручной вязки из 100% мериносовой шерсти.", ShortDescription = "Вязаный свитер, тёплый и мягкий", Category = "Вязание", Material = "Шерсть", Color = "Синий", Width = 50, Height = 70 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Керамическая кружка ручной работы", Price = 890, Quantity = 10, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Кружка из белой глины, покрытая глазурью. Объём 350 мл.", ShortDescription = "Уютная кружка из белой глины", Category = "Керамика", Material = "Глина", Color = "Белый с синим узором", Width = 10, Height = 12 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Серьги с лазуритом", Price = 1350, Quantity = 5, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Серьги ручной работы с натуральным лазуритом в оправе из серебра 925 пробы.", ShortDescription = "Серебряные серьги с лазуритом", Category = "Украшения", Material = "Серебро, лазурит", Color = "Синий", Width = 2, Height = 4 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Картина маслом «Осенний лес»", Price = 5800, Quantity = 1, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Живописная картина маслом на холсте. Рама в комплекте.", ShortDescription = "Картина маслом, осенний пейзаж", Category = "Живопись", Material = "Холст, масло", Color = "Жёлто-оранжевый", Width = 40, Height = 60 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Льняная скатерть с вышивкой", Price = 1700, Quantity = 6, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Скатерть из натурального льна с ручной вышивкой в народном стиле. Размер 150x150 см.", ShortDescription = "Льняная скатерть с вышивкой", Category = "Текстиль", Material = "Лён", Color = "Бежевый с красным", Width = 150, Height = 150 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Деревянная шкатулка с резьбой", Price = 2100, Quantity = 4, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Шкатулка из липы с ручной резьбой. Внутри обита бархатом.", ShortDescription = "Резная шкатулка из липы", Category = "Дерево", Material = "Липа, бархат", Color = "Натуральное дерево", Width = 20, Height = 10 } },
            new Product { Id = Guid.NewGuid(), SellerId = currentSeller.Id, Title = "Керамическое блюдо для сервировки", Price = 1250, Quantity = 7, IsAvailable = true, Info = new ProductInfo { Id = Guid.NewGuid(), FullDescription = "Овальное блюдо ручной лепки из шамотной глины. Размер 30x20 см.", ShortDescription = "Керамическое блюдо ручной лепки", Category = "Керамика", Material = "Шамот", Color = "Терракотовый", Width = 30, Height = 5 } }
        };

                foreach (var product in productsToSeed)
                {
                    // Проверяем, есть ли товар с таким названием (используем ! для снятия предупреждения о null)
                    var exists = await db.Products!.AnyAsync(p => p.Title == product.Title);

                    // Если товара нет - добавляем его
                    if (!exists)
                    {
                        product.Info.ProductId = product.Id;
                        db.Products!.Add(product);
                    }
                }

                // Сохраняем только то, что добавилось нового
                await db.SaveChangesAsync();
            }
        }


    }
}
