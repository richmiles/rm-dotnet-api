using System.ComponentModel.DataAnnotations.Schema;

namespace RM.Api.Data
{
    public class UserToken : ModelBase
    {
        public Guid UserId { get; set; } = new();
        public virtual User User { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpirationUtc { get; set; } = DateTime.UtcNow;
    }
}
