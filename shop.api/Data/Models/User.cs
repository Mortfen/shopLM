using Microsoft.AspNetCore.Identity;

namespace shop.api.Data.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = "";
        public string? PhoneNumber { get; set; }


        public List<Product> Products { get; set; } = new List<Product>();
        public List<Order> Orders { get; set; } = new List<Order>();

    }
}
