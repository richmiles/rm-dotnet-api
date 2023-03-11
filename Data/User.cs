using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RM.Api.Data
{
    public class User : IdentityUser<Guid>
    {
        public User() { }
        public User(
            string emailAddress,
            string nameFirst,
            string nameLast,
            DateTime dob) 
        {
            UserName = emailAddress;
            Email = emailAddress;
            NameFirst = nameFirst;
            NameLast = nameLast;
            DOB = dob;
        }

        public string NameFirst { get; set; } = string.Empty;
        public string NameLast { get; set;} = string.Empty;
        public DateTime DOB { get; set; }
    }
}
