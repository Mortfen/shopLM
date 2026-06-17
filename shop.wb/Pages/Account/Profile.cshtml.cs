using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace shop.wb.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly API api;
        private readonly IConfiguration config;

        public ProfileModel(API api, IConfiguration config)
        {
            this.api = api;
            this.config = config;
        }

        public string ApiBase => this.config["ApiBaseUrl"] ?? "https://localhost:7168";
        public ProfileDto?  Profile { get; set;}
        public List<ProductDto> MyProducts { get; set; } = new();
        public async Task OnGetAsync()
        {
            Profile = await api.GetAsync<ProfileDto>("api/users/me");

            if (User.IsInRole("Seller"))
            {
                MyProducts = await api.GetAsync<List<ProductDto>>("api/users/me/products") ?? new();
            }
        }

        [BindProperty]
        [Required(ErrorMessage = "Имя обязательно")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ\s]+$", ErrorMessage = "Имя должно содержать только буквы")]
        public string FullName { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = "";

        public async Task<IActionResult> OnPostSaveProfileAsync()
        {
            if (!ModelState.IsValid) { return Page(); }
            await this.api.PutAsync("api/users/me/change", new { FullName,Email });
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostBecomeSellerAsync()
        {
            var response = await this.api.PostAsync("api/users/me/become-seller", new { });
            if (response.IsSuccessStatusCode)
            {
                // Роль изменилась — нужен новый JWT, проще всего перелогиниться
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return RedirectToPage("/Account/Login");
            }
            return RedirectToPage();
        }

        //удаление
        public async Task<IActionResult> OnPostDeleteProductAsync(Guid id)
        {
            await this.api.DeleteAsync($"api/products/{id}");
            return RedirectToPage();
        }
    }
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public List<string> Roles { get; set; } = new();

    }
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
