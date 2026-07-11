using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SecurityExplorer.SampleWebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SecurityExplorer.SampleWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {

            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO loginDTO)
        {
            if (loginDTO.email == "admin" && loginDTO.password == "admin")
            {
                var response = GenerateJwtToken(loginDTO.email);
                return Ok(response);
            }

            return Unauthorized(new { message = "Invalid username or password credentials." });
        }

        private LoginResponseDTO GenerateJwtToken(String username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
             var expiresAt = DateTime.UtcNow.AddMinutes(15);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };


            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new LoginResponseDTO
    {
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        ExpiresAt = expiresAt
    };
        }
    }
}
