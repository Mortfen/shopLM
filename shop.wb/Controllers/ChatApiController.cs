using Microsoft.AspNetCore.Mvc;
using shop.wb.Pages;
using shop.wb.Services;

namespace shop.wb.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatApiController : ControllerBase
    {
        private readonly API _api;
        public ChatApiController(API api) => _api = api;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest req)
        {
            var response = await _api.PostAsync<ChatRequest>("api/chat", req);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
