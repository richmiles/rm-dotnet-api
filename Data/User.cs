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
            DateTime dob,
            DateTime privacyOptinDate,
            DateTime? marketingOptinDate)
        {
            UserName = emailAddress;
            Email = emailAddress;
            NameFirst = nameFirst;
            NameLast = nameLast;
            DOB = dob;
        }

        [ProtectedPersonalData]
        public string NameFirst { get; set; } = string.Empty;
        [ProtectedPersonalData]
        public string NameLast { get; set; } = string.Empty;
        [ProtectedPersonalData]
        public DateTime DOB { get; set; }
        public DateTime PrivacyOptinDate { get; set; }
        public DateTime? MarketingOptinDate { get; set; }
    }
}
