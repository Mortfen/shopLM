using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace shop.wb.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory httpFactory;
        public LoginModel(IHttpClientFactory httpFactory)
        { this.httpFactory = httpFactory;  }
        public class InputModel
        {
            
                
           

            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Введите пароль")]
            [MinLength(8, ErrorMessage = "Минимум 8 символов")]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!;$#*&@.]).+$",
                ErrorMessage = "Пароль должен содержать заглавную букву, цифру и спецсимвол")]
            public string Password { get; set; } = "";


            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            { return Page(); }    
                

            var client = this.httpFactory.CreateClient("API");

            var response = await client.PostAsJsonAsync("api/auth/login", new
            {
                Email = Input.Email,
                Password = Input.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Неверный email или пароль";
                return Page();
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            //Сохраняем токен в сессии
            HttpContext.Session.SetString("jwt", result!.Token);
            //создание cooke
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);

            var claims = jwtToken.Claims.ToList();

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = jwtToken.ValidTo, // синхронизируем срок с JWT
                    // IsPersistent = Input.RememberMe save cooke
                    IsPersistent = true
                });



            return RedirectToPage("/Index");
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = "";
    }
}

