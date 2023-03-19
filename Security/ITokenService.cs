using RM.Api.Data;
using System.Security.Claims;

namespace RM.Api.Security
{
    public interface ITokenService
    {
        Task<AuthToken> GenerateTokenAsync(User user);
        Task<AuthToken> RefreshTokenAsync(string token);
        Task<ClaimsPrincipal> ValidateTokenAsync(string authToken);
    }
}
