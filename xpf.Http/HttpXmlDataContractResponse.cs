using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace xpf.Http
{
    public class HttpJsonDataContractResponse : HttpJsonRequest
    {
        public override string Serialize<T>(T data)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                var xser = new DataContractJsonSerializer(typeof(T));
                xser.WriteObject(ms, data);
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
