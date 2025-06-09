using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.Data;
using Project.Models;
using Project.Helpers;  // הוספת ה- helper
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _ctx.Parents.FirstOrDefault(u => u.Email == req.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
                return Unauthorized("Invalid credentials.");
            var token = JwtHelper.GenerateToken(
                user.Id,
                _config["Jwt:Key"],
                expiresInHours: 2
            );
            return Ok(new { token });
        }


        public class RegisterRequest
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class ForgotRequest
        {
            public string Email { get; set; }
        }

        public class ResetRequest
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }
    }
}