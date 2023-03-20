using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace RM.Api.Security
{
    public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenService _tokenService;

        public TokenAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenService tokenService)
            : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }



        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.NoResult();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var principal = await _tokenService.ValidateTokenAsync(token);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (TokenException ex)
            {
                Logger.LogInformation(ex, "Invalid token.");
                return AuthenticateResult.Fail("Invalid token.");
            }
        }

    }
}
