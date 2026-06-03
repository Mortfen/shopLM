using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace shop.wb.Pages.Account
{
    public class LoginModel : PageModel
    {
        public class InputModel
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet() { }
        public IActionResult OnPost() => Page();
    }
}
