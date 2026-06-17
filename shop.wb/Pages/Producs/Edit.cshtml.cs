using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;
using System.ComponentModel.DataAnnotations;

namespace shop.wb.Pages.Producs
{
    public class EditModel : PageModel
    {
        private readonly API api;

    public EditModel(API api) { this.api = api; }

        [BindProperty]
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "От 2 до 200 символов")]
        public string Name { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Категория обязательна")]
        public string Category { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "От 10 до 2000 символов")]
        public string Description { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Укажите материал")]
        public string Material { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Укажите цвет")]
        public string Color { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Укажите цену")]
        [Range(1, 1000000, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Укажите количество")]
        [Range(1, 9999, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Укажите высоту")]
        [Range(0, 9999, ErrorMessage = "Это не воздух, укажите зарактеристики")]
        public int Width { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Укажите вес")]
        [Range(0, 9999, ErrorMessage = "Это не воздух, укажите зарактеристики")]
        public int Height { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            var product = await this.api.GetAsync<ProductDetailDto>($"api/products/{id}");
            if (product == null) return;

            Name = product.Title;
            Price = product.Price;
            Quantity = product.Quantity;
            Category = product.Info?.Category ?? "";
            Description = product.Info?.FullDescription ?? "";
            Material = product.Info?.Material ?? "";
            Color = product.Info?.Color ?? "";
            Width = product.Info?.Width ?? 0;
            Height = product.Info?.Height ?? 0;
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid) { return Page(); }
            await this.api.PutAsync($"api/products/{id}", new
            {
                Name,
                Category,
                Description,
                Material,
                Color,
                Width,
                Height,
                Price,
                Quantity
            });
            return RedirectToPage("/Account/Profile");
        }

        public class ProductDetailDto
        {
            public string Title { get; set; } = "";
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public ProductInfoDto? Info { get; set; }
        }
        public class ProductInfoDto
        {
            public string? FullDescription { get; set; }
            public string? Category { get; set; }
            public string? Material { get; set; }
            public string? Color { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
        }
    }
}
