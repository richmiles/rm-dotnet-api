using System.Text.RegularExpressions;

namespace RM.Api.Data
{
    public static partial class Utilities
    {
        public static string NormalizeEmailAddress(string email)
        {
            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentNullException(nameof(email));
            }

            email = email.Trim();
            if (string.IsNullOrEmpty(email)) {
                throw new ArgumentNullException(nameof(email));
            }

            email = email.ToLower();
            var atIndex = email.IndexOf('@');

            if (atIndex < 1 || atIndex == email.Length - 1)
            {
                throw new ArgumentException("Invalid email address.", nameof(email));
            }

            var localPart = email.Substring(0, atIndex);
            var domainPart = email.Substring(atIndex + 1);

            localPart = EmailNormalizationRegex().Replace(localPart, "");

            return $"{localPart}@{domainPart}";
        }

        [GeneratedRegex("[^\\w\\.\\+\\-]+", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex EmailNormalizationRegex();
    }
}
