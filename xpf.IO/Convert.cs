using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace xpf.IO
{
    public static class Convert
    {
        public static string ToXml<T>(this T instance, params Type[] extraTypes)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                var xser = new XmlSerializer(typeof (T), extraTypes);
                xser.Serialize(ms, instance);

                ms.Position = 0;
                using (var s = new StreamReader(ms))
                {
                    xml = s.ReadToEnd();
                }
            }
            return xml;
        }

        public static T FromXmlToInstance<T>(this string xml, params Type[] extraTypes)
            where T : class
        {
            T entity;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var xser = new XmlSerializer(typeof (T), extraTypes);
                entity = xser.Deserialize(ms) as T;
            }
            return entity;
        }

        public static T FromJsonToInstance<T>(this string json)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
