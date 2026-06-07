using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        // POST api/auth/register
        [HttpPost("api/auth/register")]
        public IActionResult Register()
        {
            throw new NotImplementedException();
        }

        // POST api/auth/login
        [HttpPost("api/auth/login")]
        public IActionResult Login()
        {
            throw new NotImplementedException();
        }

        // POST api/auth/refresh
        [HttpPost("api/auth/refresh")]
        public IActionResult Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
