using System.Collections.Generic;

namespace xpf.Http
{
    public class HttpRequest
    {
        public HttpRequest()
        {
            this.Headers = new List<HttpHeader>();
            this.Cookies = new List<HttpCookie>();
            this.RequestFormat = HttpFormat.Text;
            this.Headers.Add(new HttpHeader {Key = "Accept", Value = new List<string> {"text/html"}});
            this.Headers.Add(new HttpHeader {Key = "Content-Type", Value = new List<string> {"text/html"}});
            //this.Headers.Add(new HttpHeader {Key = "Accept-Encoding", Value = new List<string> {"gzip, deflate"}});
            this.AllowAutoRedirect = true;
        }

        public string Url { get; set; }

        public HttpFormat RequestFormat { get; protected set; }

        public AuthenticationToken AuthenticationToken { get; set; }

        public List<HttpHeader> Headers { get; private set; }

        public List<HttpCookie> Cookies { get; private set; }
        public virtual string Serialize<T>(T data)
        {
            return data as string;
        }

        public bool AllowAutoRedirect { get; set; }
    }
}