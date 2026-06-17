using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shop.api.Data.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserManager<User> userManager;
        private readonly DB db;

        public UsersController(UserManager<User> userManager, DB db)
        {
            this.userManager = userManager;
            this.db = db;
        }



        // GET api/users/me — мой профиль
        [Authorize]
        [HttpGet("api/users/me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) { return Unauthorized(); }


            var user = await userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }
            var roles = await userManager.GetRolesAsync(user);

            return Ok(new {user.Id, user.Email, user.FullName, Roles = roles });
        }

        // PUT api/users/me — редактировать профиль
       // [HttpPut("api/users/me")]
        //public IActionResult UpdateProfile()
        //{
          //  throw new NotImplementedException();
        //}

        // POST api/users/me/become-seller — стать продавцом
 

        [Authorize]
        [HttpPost("api/users/me/become-seller")]
        public async Task<IActionResult> BecomeSeller()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await this.userManager.FindByIdAsync(userId);

            if (await this.userManager.IsInRoleAsync(user!, "Seller"))
                return BadRequest("Уже является продавцом");

            await this.userManager.AddToRoleAsync(user!, "Seller");
            return Ok();
        }





        [Authorize]
        [HttpGet("api/users/me/products")]
        public async Task<IActionResult> GetMyProduct()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var products = await db.Products
    .Include(p => p.Info)
    .Include(p => p.Images) // ← добавить
    .Where(p => p.SellerId == userId)
    .ToListAsync();

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Title,
                p.Price,
                p.Quantity,
                ImageUrl = p.Images.FirstOrDefault()?.ImageUrl // ← добавить
            }));
        }
        [HttpGet("api/debug")]
        public IActionResult Debug()
        {
            var auth = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"=== API DEBUG: Authorization header = '{auth}'");
            Console.WriteLine($"=== API DEBUG: IsAuthenticated = {User.Identity?.IsAuthenticated}");
            return Ok(new
            {
                authHeader = auth,
                isAuthenticated = User.Identity?.IsAuthenticated,
                hasToken = !string.IsNullOrEmpty(auth)
            });
        }
    }
}
