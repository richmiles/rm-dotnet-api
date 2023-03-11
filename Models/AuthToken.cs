using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rm_dotnet_api.Models
{
    public class AuthToken
    {
        public Guid Id { get; set; } = default;
        public string Email { get; set; } = string.Empty;
        public string NameFirst { get; set; } = string.Empty;
        public string NameLast { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
