﻿using System.Collections.Generic;
using System.IO;

namespace xpf.Http
{
    public class HttpXmlRequest : HttpRequest   
    {
        public HttpXmlRequest()
        {
            this.RequestFormat = HttpFormat.XML;
            this.Headers.Add(new HttpHeader { Key = "Accept", Value = new List<string> { "text/html" } });
            this.Headers.Add(new HttpHeader { Key = "Accept", Value = new List<string> { "application/xml" } });
            this.Headers.Add(new HttpHeader { Key = "Content-Type", Value = new List<string> { "application/xml" } });
        }

        public override string Serialize<T>(T data)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                XmlSerializer.SerializeToStream(ms, data);
                ms.Position = 0;
                using (var s = new StreamReader(ms))
                {
                     xml = s.ReadToEnd();
                }
            }
            return xml;
        }
    }
}