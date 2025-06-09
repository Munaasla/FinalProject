using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Project.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(int userId, string secretKey, int expiresInHours)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                       new System.Security.Claims.Claim("userId", userId.ToString())
                   }),
                Expires = DateTime.Now.AddHours(expiresInHours),
                Issuer = "yourIssuer", 
                Audience = "yourAudience",  
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); 
        }
    }
}
