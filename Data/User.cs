using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RM.Api.Data
{
    public class User : ModelBase
    {
        public User() { }
        public User(
            string emailAddress,
            string passwordHash,
            string nameFirst,
            string nameLast,
            DateTime dob,
            DateTime privacyOptinDate,
            DateTime? marketingOptinDate)
        {
            Email = emailAddress;
            NormalizedEmailAddress = Utilities.NormalizeEmailAddress(emailAddress);
            PasswordHash = passwordHash;
            NameFirst = nameFirst;
            NameLast = nameLast;
            DOB = dob;
            PrivacyOptinDate = privacyOptinDate;
            MarketingOptinDate = marketingOptinDate;
        }

        [ProtectedPersonalData]
        public string Email { get; set; } = string.Empty;

        [ProtectedPersonalData]
        public string NormalizedEmailAddress { get; set; } = string.Empty;
        [ProtectedPersonalData]
        public string PasswordHash { get; set; } = string.Empty;

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
