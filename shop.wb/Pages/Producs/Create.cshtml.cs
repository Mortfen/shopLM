using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace shop.wb.Pages.Producs
{
    public class DataProduct
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string Color { get; set; } = "";
        public string Material { get; set; } = "";
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }

   // [Authorize(Roles ="Seller")]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public DataProduct ProductINFO { get; set; } = new();

        [BindProperty]
        public List<IFormFile> Photos { get; set; } = new();

        public void OnGet() { }
        public IActionResult OnPost() => Page();
    }
}

