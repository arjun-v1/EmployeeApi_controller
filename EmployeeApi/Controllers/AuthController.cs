using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Employee_Common_Lib.Manager;
using Employee_Common_Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginManager _loginManager;
        private readonly IConfiguration _configuration;

        public AuthController(ILoginManager loginManager, IConfiguration configuration)
        {
            _loginManager = loginManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            var login = new Login
            {
                Email = request.Email,
                Password = request.Password,
               
            };

            var result = _loginManager.CreateLogin(login);
            return Ok("User registered successfully");
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _loginManager.ValidateUser(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                Email = user.Email,
                
            });
        }

        private string GenerateJwtToken(Login user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    //new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString())
                },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}




