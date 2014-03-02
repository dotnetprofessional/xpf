using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace xpf.Http
{
    public class HttpResponse<T>
    {
        UriDetail _detail;

        public HttpResponse(string url, HttpStatusCode statusCode, T content, string error, string rawContent)
        {
            this.Headers = new Dictionary<string, HttpHeader>();
            StatusCode = statusCode;
            Content = content;
            Error = error;
            this.Url = url;
            this.RawContent = rawContent;
        }

        public string Url { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public T Content { get; private set; }

        public string Error { get; set; }

        public Dictionary<string, HttpHeader> Headers { get; set; }

        public string RawContent { get; private set; }

        public UriDetail Detail
        {
            get
            {
                if (_detail == null)
                    _detail = new UriDetail(this.Url, this.RawContent);

                return _detail;
            }
            private set { _detail = value; }
        }
    }
}