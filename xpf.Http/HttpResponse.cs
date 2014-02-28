using System.Collections.Generic;
using System.Net;

namespace xpf.Http
{
    public class HttpResponse<T>
    {
        public HttpResponse(HttpStatusCode statusCode, T content, string error)
        {
            this.Headers = new Dictionary<string, HttpHeader>();
            StatusCode = statusCode;
            Content = content;
            Error = error;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public T Content { get; private set; }

        public string Error { get; set; }

        public Dictionary<string, HttpHeader> Headers { get; set; }
    }
}