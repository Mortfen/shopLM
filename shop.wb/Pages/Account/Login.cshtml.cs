using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace shop.wb.Pages.Account
{
    public class LoginModel : PageModel
    {
        public class InputModel
        {
            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Введите пароль")]
            [MinLength(8, ErrorMessage = "Минимум 8 символов")]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!;$#*&@.]).+$",
                ErrorMessage = "Пароль должен содержать заглавную букву, цифру и спецсимвол")]
            public string Password { get; set; }


            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet() { }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return StatusCode(401);


            return Page();
            // логика логина
        }
    }
}
