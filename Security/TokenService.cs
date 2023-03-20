using Microsoft.EntityFrameworkCore;
using RM.Api.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RM.Api.Security
{
    public class TokenService : ITokenService
    {
        private readonly IAppDbContextFactory _dbContextFactory;

        public TokenService(IAppDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
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
                ExpirationUtc = DateTime.UtcNow.AddMinutes(10)
            };

            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.UserTokens.Add(userToken);
            await dbContext.SaveChangesAsync();

            return new AuthToken(token, (int)(userToken.ExpirationUtc - DateTime.UtcNow).TotalSeconds);
        }

        public async Task<AuthToken> RefreshTokenAsync(string existingToken)
        {
            // Find the user token in the database
            using var dbContext = _dbContextFactory.CreateDbContext();
            var userToken = await dbContext.UserTokens
                .FirstOrDefaultAsync(ut => ut.Token == existingToken);

            if (userToken == null)
            {
                throw new TokenException("Invalid or expired token.");
            }

            if(userToken.ExpirationUtc < DateTime.UtcNow.AddDays(2)) 
            {
                // Delete the existing User Token
                dbContext.Remove(existingToken);
                await dbContext.SaveChangesAsync();
                throw new TokenException("Token is not eligible for refresh."); 
            }

            // Delete the existing User Token
            dbContext.Remove(existingToken);
            await dbContext.SaveChangesAsync();

            // Generate a cryptographically strong random token
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenBytes = new byte[32]; // 256 bits
            rng.GetBytes(tokenBytes);
            string newToken = Convert.ToBase64String(tokenBytes);
            var newUserToken = new UserToken
            {
                UserId = userToken.UserId,
                Token = newToken,
                ExpirationUtc = DateTime.UtcNow.AddMinutes(10)
            };

            // Return the new token value
            return new AuthToken(newToken, (int)(newUserToken.ExpirationUtc - DateTime.UtcNow).TotalSeconds);
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string authToken)
        {
            // Find the token in the UserTokens DbSet
            using var dbContext = _dbContextFactory.CreateDbContext();
            var userToken = await dbContext.UserTokens
                .FirstOrDefaultAsync(ut => ut.Token == authToken && ut.ExpirationUtc > DateTime.UtcNow);

            if (userToken == null)
            {
                throw new TokenException("User Token not found.");
            }

            var user = await dbContext.Users.FirstAsync(u => u.Id == userToken.UserId);
            // Create a ClaimsIdentity with the user's claims
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userToken.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var identity = new ClaimsIdentity(claims, "Token");
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
