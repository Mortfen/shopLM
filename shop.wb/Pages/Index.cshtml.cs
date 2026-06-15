using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;

namespace shop.wb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly API aPI;

        public IndexModel(API aPI)
        {
            this.aPI = aPI;
        }
       
     
        public void OnGet()
        {
        }
    }
}
