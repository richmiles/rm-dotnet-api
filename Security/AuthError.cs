namespace RM.Api.Security
{
    public class AuthError
    {
        public AuthError() { }
        public AuthError(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
