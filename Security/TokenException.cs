namespace RM.Api.Security
{
    public class TokenException : Exception
    {
        public TokenException(string message) : base(message)
        {
        }
    }
}
