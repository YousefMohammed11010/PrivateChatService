using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PrivateChatService.Data;
using PrivateChatService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrivateChatService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            var user = new User { Email = model.Email, Password = model.Password };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("User registered");
        }

        [HttpPost("login")]
        public IActionResult Login(RegisterModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (user == null) return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}