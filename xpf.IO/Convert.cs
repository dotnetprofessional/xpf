using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

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
            return new MemoryStream(Encoding.UTF8.GetBytes(xml)).FromXmlStreamToInstance<T>();
        }

        public static T FromXmlStreamToInstance<T>(this Stream stream, params Type[] extraTypes) 
            where T : class
        {
            T entity;
            using (stream)
            {
                var xser = new XmlSerializer(typeof(T), extraTypes);
                entity = xser.Deserialize(stream) as T;
            }
            return entity;
        }
}
}
