using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RM.Api.Data;
using System.Security.Claims;

namespace RM.Api.Security
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet()]
        public async Task<ActionResult<AuthToken>> UserAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) { return Unauthorized(); }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userIdClaim.Value));
            return Ok(user);
        }
    }
}
