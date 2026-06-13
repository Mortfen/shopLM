using shop.api.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using shop.api.Data.Models;

namespace shop.api.Data
{
    public class DB : IdentityDbContext<User, IdentityRole<Guid>,Guid>
    {


        public DB(DbContextOptions<DB> options) : base (options) { }

        
        public DbSet<Product>? Products { get; set; }
        public DbSet<ProductInfo>? ProductInfos { get; set; }
        public DbSet<ProductImage>? ProductImages { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderItems>? OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            

            base.OnModelCreating(builder);
            builder.Entity<Product>().HasOne(x => x.Seller).WithMany(u => u.Products).HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ProductInfo>().HasOne(x => x.Product).WithOne(x => x.Info).HasForeignKey<ProductInfo>(pi => pi.ProductId);
            builder.Entity<ProductImage>().HasOne(img => img.Product).WithMany(p => p.Images).HasForeignKey(img => img.ProductId);
            builder.Entity<Order>().HasOne(o => o.Buyer).WithMany(u => u.Orders).HasForeignKey(o => o.BuyerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
            builder.Entity<OrderItems>().HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            builder.Entity<OrderItems>().HasOne(oi => oi.Product).WithMany(p => p.OrderItems).HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
            //seed role
            var buyerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var sellerId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var adminId = Guid.Parse("10000000-0000-0000-0000-000000000003");
            //identity role
            builder.Entity<IdentityRole<Guid>>().HasData(
           new IdentityRole<Guid> { Id = buyerId, Name = "Buyer", NormalizedName = "BUYER", ConcurrencyStamp = "11111111-1111-1111-1111-111111111111" },
           new IdentityRole<Guid> { Id = sellerId, Name = "Seller", NormalizedName = "SELLER", ConcurrencyStamp = "22222222-2222-2222-2222-222222222222" },
           new IdentityRole<Guid> { Id = adminId, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "33333333-3333-3333-3333-333333333333" }
                );
        }

    }
}
