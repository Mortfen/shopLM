using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace shop.wb.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;

        public RegisterModel(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Введите имя")]
            public string FullName { get; set; } = "";

            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Введите пароль")]
            [MinLength(8, ErrorMessage = "Минимум 8 символов")]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!;$#*&@.]).+$",
                ErrorMessage = "Пароль должен содержать заглавную букву, цифру и спецсимвол")]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Повторите пароль")]
            [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = "";
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpFactory.CreateClient("API");

            var response = await client.PostAsJsonAsync("api/auth/register", new
            {
                Email = Input.Email,
                Password = Input.Password,
                FullName = Input.FullName
            });

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Не удалось зарегистрироваться. Возможно email уже используется.";
                return Page();
            }

            return RedirectToPage("Login");
        }
    }
}
