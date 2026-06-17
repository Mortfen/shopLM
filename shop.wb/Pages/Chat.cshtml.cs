using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using shop.wb.Services;

namespace shop.wb.Pages
{
    [Authorize]
    public class ChatModel : PageModel
    {
        private readonly API _api;
        public ChatModel(API api) => _api = api;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync([FromBody] ChatRequest req)
        {
            var response = await _api.PostAsync<ChatRequest>("api/chat", req);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }

    public record ChatRequest(string Message);
}


