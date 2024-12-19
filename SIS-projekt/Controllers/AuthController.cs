using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIS_projekt.DTO;
using SIS_projekt.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIS_projekt.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Login(LoginUserDTO dto)
        {
            try
            {
                User? user = _context.Users.FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);
                if (user is null)
                {
                    return BadRequest();
                }

                string token = GenerateJwtToken(user.Id);
                Response.Headers.Append("Authorization", $"Bearer {token}");
                return Ok("Login Successful");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(RegisterUserDTO dto)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is not null)
            {
                return BadRequest("Email taken");
            }
            try
            {
                User newUser = new()
                {
                    Email = dto.Email,
                    Password = dto.Password
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(UsersController.GetUser), "Users", new { id =  newUser.Id }, newUser.Email);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJwtToken(int id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []{new Claim("id", id.ToString())}),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["JWT:Audience"],
                Issuer = _configuration["JWT:Issuer"],
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
