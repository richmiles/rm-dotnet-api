using Microsoft.EntityFrameworkCore;
using RM.Api.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RM.Api.Security
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _context;

        public TokenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthToken> GenerateTokenAsync(User user)
        {
            // Generate a cryptographically strong random token
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenBytes = new byte[32]; // 256 bits
            rng.GetBytes(tokenBytes);
            string token = Convert.ToBase64String(tokenBytes);

            // Save the token in the UserTokens DbSet with an expiration time
            UserToken userToken = new()
            {
                UserId = user.Id,
                Token = token,
                ExpirationUtc = DateTime.UtcNow.AddDays(2)
            };

            _context.UserTokens.Add(userToken);
            await _context.SaveChangesAsync();

            return new AuthToken(token, userToken.ExpirationUtc);
        }

        public async Task<AuthToken> RefreshTokenAsync(string existingToken)
        {
            // Find the user token in the database
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.Token == existingToken && ut.ExpirationUtc > DateTime.UtcNow);

            if (userToken == null)
            {
                throw new TokenException("Invalid or expired token.");
            }

            // Expire the existing User Token
            userToken.ExpirationUtc = DateTime.UtcNow;
            _context.Update(userToken);
            await _context.SaveChangesAsync();

            // Generate a cryptographically strong random token
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenBytes = new byte[32]; // 256 bits
            rng.GetBytes(tokenBytes);
            string newToken = Convert.ToBase64String(tokenBytes);
            var newUserToken = new UserToken
            {
                UserId = userToken.UserId,
                Token = newToken,
                ExpirationUtc = DateTime.UtcNow.AddDays(2)
            };

            // Return the new token value
            return new AuthToken(newToken, newUserToken.ExpirationUtc);
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string authToken)
        {
            // Find the token in the UserTokens DbSet
            var userToken = await _context.UserTokens
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(ut => ut.Token == authToken && ut.ExpirationUtc > DateTime.UtcNow);

            if (userToken == null)
            {
                throw new TokenException("User Token not found.");
            }

            // Create a ClaimsIdentity with the user's claims
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userToken.UserId.ToString()),
            new Claim(ClaimTypes.Email, userToken.User.Email)
        };

            var identity = new ClaimsIdentity(claims, "Token");
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
