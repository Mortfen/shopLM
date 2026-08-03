using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using shop.wb.Services;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace shop.wb.Pages.Producs
{
    public class DataProduct
    {
        [Required(ErrorMessage = "Введите название")]
        [RegularExpression(@"^[a-zA-Z\u0400-\u04FF\s]+$", ErrorMessage = "Only letters allowed")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Выберите категорию")]
        [RegularExpression(@"^[a-zA-Z\u0400-\u04FF\s]+$", ErrorMessage = "Only letters allowed")]
        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Описание не доваленно")]
        public string Description { get; set; } = ""; // описание не трогаем - там могут быть цифры и знаки

        [Required(ErrorMessage = "Цвет, или это хамельон?")]
        [RegularExpression(@"^[a-zA-Z\u0400-\u04FF\s]+$", ErrorMessage = "Only letters allowed")]
        public string Color { get; set; } = "";

        [Required(ErrorMessage = "Укажите материал")]
        [RegularExpression(@"^[a-zA-Z\u0400-\u04FF\s]+$", ErrorMessage = "Only letters allowed")]
        public string Material { get; set; } = "";

        [Required(ErrorMessage = "Укажите ширину")]
        [Range(1, double.MaxValue, ErrorMessage = "Ширина не может быть меньше нуля")]
        public int Width { get; set; }

        [Required(ErrorMessage = "Укажите длину")]
        [Range(1, double.MaxValue, ErrorMessage = "Введите корректную длину")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Укажите цену")]
        [Range(1, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Укажите количество")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; } = 1;
    }

    [Authorize(Roles ="Seller")]
    [Authorize]
        public class CreateModel : PageModel
        {
            private readonly API api;
            private readonly IHttpClientFactory httpFactory;

            public CreateModel(API api, IHttpClientFactory httpFactory)
            {
                this.api = api;
                this.httpFactory = httpFactory;
            }

            [BindProperty]
            public DataProduct Product { get; set; } = new();

            [BindProperty]
            public List<IFormFile> Photos { get; set; } = new();

            public string? ErrorMessage { get; set; }

            public void OnGet() { }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid) return Page();

            var token = HttpContext.Session.GetString("jwt");
            var client = httpFactory.CreateClient("API");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }



            var response = await client.PostAsJsonAsync("api/products", Product);

            if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Не удалось создать товар";
                    return Page();
                }

                var created = await response.Content.ReadFromJsonAsync<CreatedProductDto>();

                var realPhotos = Photos.Where(p => p.Length > 0).ToList();
                if (realPhotos.Any() && created != null)
                {
                    await UploadPhotosAsync(created.Id, realPhotos);
                }

                return RedirectToPage("/Account/Profile");
            }

            private async Task UploadPhotosAsync(Guid productId, List<IFormFile> photos)
            {
                using var content = new MultipartFormDataContent();

                foreach (var photo in photos)
                {
                    var streamContent = new StreamContent(photo.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                    content.Add(streamContent, "files", photo.FileName);
                }

                var token = HttpContext.Session.GetString("jwt");
                var client = httpFactory.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                await client.PostAsync($"api/products/{productId}/images", content);
            }
        }

        public class CreatedProductDto
        {
            public Guid Id { get; set; }
        }


}


