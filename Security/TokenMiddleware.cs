namespace RM.Api.Security
{
    public class TokenMiddleware
    {
        // Create middleware for authenticating and authorizing
        // an opaque token.
        public TokenMiddleware(
            RequestDelegate next,
            ITokenService tokenService,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _tokenService = tokenService;
            _logger = loggerFactory.CreateLogger<TokenMiddleware>();
        }

        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;

        public async Task Invoke(HttpContext context)
        {
            // Check for the presence of the Authorization header.
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogInformation("Authorization header not found.");
                await _next.Invoke(context);
                return;
            }

            // Check for the presence of the Bearer scheme.
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Bearer "))
            {
                _logger.LogInformation("Bearer scheme not found.");
                await _next.Invoke(context);
                return;
            }

            // Extract the token from the Authorization header.
            var token = authHeader.Substring("Bearer ".Length).Trim()!;

            // Create a ClaimsPrincipal from the token.
            var principal = await _tokenService.ValidateTokenAsync(token);

            // Attach the ClaimsPrincipal to the HttpContext.
            context.User = principal;

            // Call the next delegate/middleware in the pipeline.
            await _next.Invoke(context);
        }
    }
}
