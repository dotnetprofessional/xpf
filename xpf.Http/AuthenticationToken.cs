namespace xpf.Http
{
    public class AuthenticationToken
    {
        public AuthenticationToken(string scheme, string token)
        {
            Scheme = scheme;
            Value = token;
        }

        public string Scheme { get; protected set; }

        public string Value { get; protected set; }
    }
}