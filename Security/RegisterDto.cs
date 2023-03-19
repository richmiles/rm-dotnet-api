using System.ComponentModel.DataAnnotations;

namespace RM.Api.Security
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string NameFirst { get; set; } = string.Empty;
        [Required]
        public string NameLast { get; set; } = string.Empty;
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public bool PrivacyOptin { get; set; }
        [Required]
        public bool MarketingOptin { get; set; }
    }
}
