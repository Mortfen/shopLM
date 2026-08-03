using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using shop.api.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RSA privateKey;
        public AuthController(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.privateKey = RSA.Create();
            this.privateKey.ImportFromPem(System.IO.File.ReadAllText("key/key.pem")); //=>>FILE error compilator
                                                                                       // _privateKey.ImportFromEncryptedPem if pass on pem
        }
        // POST api/auth/register
        [HttpPost("api/auth/register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User { Email = dto.Email, UserName = dto.Email, FullName = dto.FullName };
            var result = await this.userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await this.userManager.AddToRoleAsync(user, "Buyer");

            return Ok();
        }

        // POST api/auth/login
        [HttpPost("api/auth/login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await this.userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Неверный логин или пароль");

            var valid = await this.userManager.CheckPasswordAsync(user, dto.Password);
            if (valid == false) return Unauthorized("Неверный логин или пароль");

            var roles = await this.userManager.GetRolesAsync(user);
            var token = GenerateToken(user, roles);
            return Ok(new { token });
        }

        //метод jwt
        private string GenerateToken(User user, IEnumerable<string> roles)
        {
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName)
            };
            foreach (var role in roles) 
            { 
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var credentialis = new SigningCredentials(new RsaSecurityKey(this.privateKey),SecurityAlgorithms.RsaSha256);
            var token = new JwtSecurityToken
                (
                    issuer:"HobbyShop.api",
                    audience:"HobbyShop.Web",
                    claims: claims,
                    expires:DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentialis
                );
           return new JwtSecurityTokenHandler().WriteToken(token);



        }


    }
    



    //прослойка
    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    
}
