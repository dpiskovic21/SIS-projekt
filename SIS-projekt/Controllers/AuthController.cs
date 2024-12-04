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

        public AuthController(AppDBContext context)
        {
            _context = context;
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
                return Ok(token);

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

                return CreatedAtAction(nameof(UsersController.GetUser), "Users", new { id =  newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJwtToken(int id)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("S2FxZoeMrlu0GwoNoFNLsx0SwIIVDXFF"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: new List<Claim>(id),
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
