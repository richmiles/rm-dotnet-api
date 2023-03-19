using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RM.Api.Data;
using RM.Api.Security;

namespace RM.Api.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthToken>> LoginAsync([FromBody] LoginDto model)
        {
            if (string.IsNullOrEmpty(model.Email)) { return Unauthorized(); }

            string normalizedEmail;
            try { normalizedEmail = Utilities.NormalizeEmailAddress(model.Email); }
            catch { return Unauthorized(); }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmailAddress == normalizedEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) return Unauthorized();

            var token = await _tokenService.GenerateTokenAsync(user);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthToken>> RegisterAsync([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!model.PrivacyOptin)
            {
                return BadRequest(new List<AuthError> { new("PrivacyOptinRequired", "Privacy opt-in required") });
            }

            string normalizedEmail;
            try { normalizedEmail = Utilities.NormalizeEmailAddress(model.Email); }
            catch { return BadRequest(new List<AuthError> { new("InvalidEmail", "Email must be valid") }); }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmailAddress == normalizedEmail);
            if (existingUser != null)
            {
                return BadRequest(new List<AuthError> { new("EmailAlreadyRegistered", "Email already registered") });
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);

            var user = new User(
                model.Email,
                hashedPassword,
                model.NameFirst,
                model.NameLast,
                model.DOB,
                privacyOptinDate: DateTime.UtcNow,
                marketingOptinDate: model.MarketingOptin ? DateTime.UtcNow : null);

            _context.Add(user);
            await _context.SaveChangesAsync();

            var token = await _tokenService.GenerateTokenAsync(user);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] AuthToken model)
        {
            string existingToken = model.Token;

            if (string.IsNullOrEmpty(existingToken))
            {
                return BadRequest(new List<AuthError> { new("AuthTokenRequired", "Authorization token required") });
            }

            try
            {
                var newToken = await _tokenService.RefreshTokenAsync(existingToken);
                return Ok(newToken);
            }
            catch (TokenException ex)
            {
                return BadRequest(new List<AuthError> { new("InvalidToken", ex.Message) });
            }
        }
    }
}
