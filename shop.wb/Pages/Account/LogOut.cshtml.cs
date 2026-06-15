using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace shop.wb.Pages.Account
{
    public class LogOutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync() 
        {

            HttpContext.Session.Clear();//удаления jwt
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//удалене cooke
            return RedirectToPage("/Index");
        }
    }
}
