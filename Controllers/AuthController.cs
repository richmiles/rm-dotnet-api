using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RM.Api.Data;
using rm_dotnet_api.Models;

namespace RM.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            AppDbContext context,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model.Username.IsNullOrEmpty()) { return Unauthorized(); }

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {

                Id = user.Id!,
                Email = user.Email!,
                NameFirst = user.NameFirst!,
                NameLast = user.NameLast!,
                DOB = user.DOB,
                Token = new JwtSecurityTokenHandler().WriteToken(token)

            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User(model.Email, model.NameFirst, model.NameLast, model.DateOfBirth);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]!);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    Id = user.Id!,
                    Email = user.Email!,
                    NameFirst = user.NameFirst!,
                    NameLast = user.NameLast!,
                    DOB = user.DOB,
                    Token = tokenHandler.WriteToken(token)
                });
            }

            return BadRequest(result.Errors);
        }
    }
}