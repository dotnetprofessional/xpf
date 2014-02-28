namespace xpf.Http
{
    public class BasicAuthenticationToken : AuthenticationToken
    {
        public BasicAuthenticationToken(string token) : base("basic", token)
        {
        }
    }
}