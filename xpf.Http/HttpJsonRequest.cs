using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace xpf.Http
{
    public class HttpJsonRequest : HttpRequest
    {
        public HttpJsonRequest()
        {
            this.RequestFormat = HttpFormat.JSON;
            this.Headers.Add(new HttpHeader { Key = "Accept", Value = new List<string> { "application/json" } });
            this.Headers.Add(new HttpHeader { Key = "Content-Type", Value = new List<string> { "application/json" } });
        }

        public override string Serialize<T>(T data)
        {
            var ser = new DataContractJsonSerializer(data.GetType());
            string json = "";
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, data);
                var bytes = ms.ToArray();
                json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            return json;
        }
    }
}