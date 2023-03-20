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
            int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }
        public string Token { get; set; }
        public long ExpiresIn { get; set; }
    }
}
