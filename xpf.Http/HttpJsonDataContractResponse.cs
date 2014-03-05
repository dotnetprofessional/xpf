using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace xpf.Http
{
    public class HttpXmlDataContractResponse : HttpXmlRequest
    {
        public override string Serialize<T>(T data)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                var xser = new DataContractSerializer(typeof(T));
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
