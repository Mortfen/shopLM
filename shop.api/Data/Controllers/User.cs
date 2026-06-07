using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET api/users/me — мой профиль
        [HttpGet("api/users/me")]
        public IActionResult GetProfile()
        {
            throw new NotImplementedException();
        }

        // PUT api/users/me — редактировать профиль
        [HttpPut("api/users/me")]
        public IActionResult UpdateProfile()
        {
            throw new NotImplementedException();
        }

        // POST api/users/me/become-seller — стать продавцом
        [HttpPost("api/users/me/become-seller")]
        public IActionResult BecomeSeller()
        {
            throw new NotImplementedException();
        }

        // GET api/users/{id}/products — товары продавца
        [HttpGet("api/users/{id}/products")]
        public IActionResult GetSellerProducts(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
