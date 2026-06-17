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
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Выберите категорию")]
        public string Category { get; set; } = "";
        [Required(ErrorMessage = "Описание,не доваленно")]
        public string Description { get; set; } = "";
        [Required(ErrorMessage = "Цвет, или это хамельон?")]
        public string Color { get; set; } = "";
        public string Material { get; set; } = "";
        [Range(1, double.MaxValue, ErrorMessage = "Ширина не омжет быть меньше нуля, ты не атом")]
        public int Width { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Введите корректную длину, хватит прикалываться")]
        public int Height { get; set; }
        [Range(1,double.MaxValue,ErrorMessage = "Цена должна быть больше 0, это не благотворительность")]
        public decimal Price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0, воздух не продаём")]
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


