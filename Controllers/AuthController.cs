﻿using System;
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
            if (model.Email.IsNullOrEmpty()) { return Unauthorized(); }

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) { return Unauthorized(); }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) { return Unauthorized(); }

            var token = GenerateJwtToken(user);
            return Ok(BuildUserResponse(user, token)); ;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.PrivacyOptin == false)
            {
                return BadRequest("Privacy Optin is required");
            }

            var user = new User(
                model.Email,
                model.NameFirst,
                model.NameLast,
                model.DOB,
                privacyOptinDate: DateTime.UtcNow,
                marketingOptinDate: model.MarketingOptin ? DateTime.UtcNow : null);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var token = GenerateJwtToken(user);

                return Ok(BuildUserResponse(user, token));
            }

            return BadRequest(result.Errors);
        }

        private JwtSecurityToken GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
        }

        private object BuildUserResponse(User user, JwtSecurityToken token)
        {
            return new
            {
                user.Id,
                user.Email,
                user.NameFirst,
                user.NameLast,
                user.DOB,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

    }


}