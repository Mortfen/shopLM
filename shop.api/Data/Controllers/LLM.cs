using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class ChatController : ControllerBase
    {
        // POST api/chat — поиск через Ollama
        [HttpPost("api/chat")]
        public IActionResult Search()
        {
            throw new NotImplementedException();
        }
    }
}
