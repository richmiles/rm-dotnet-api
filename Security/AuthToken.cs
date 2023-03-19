using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RM.Api.Security
{
    public class AuthToken
    {
        public AuthToken(string token,
            DateTime expirationDateTimeUtc)
        {
            Token = token;
            ExpirationDateTimeUtc = expirationDateTimeUtc;
        }
        public string Token { get; set; }
        public DateTime ExpirationDateTimeUtc { get; set; }
    }
}
