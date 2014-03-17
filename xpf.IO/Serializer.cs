using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace xpf.IO
{
    // Serializer().AsXml<T>(object)
    // Serializer().AsJson<T>(object)
    // Serializer().FromJson<T>(object)
    public class Serializer
    {
        public static string Serialize<T>(T data)
        {
            string xml = "";
            using (var ms = new MemoryStream())
            {
                SerializeToStream(ms, data);
                ms.Position = 0;
                using (var s = new StreamReader(ms))
                {
                    xml = s.ReadToEnd();
                }
            }
            return xml;
        }

        public static T Deserialize<T>(string xml, params Type[] extraTypes)
            where T:class 
        {
            T entity;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                entity = DeserializeFromStream<T>(ms, extraTypes);
            }
            return entity;
        }

        public static T DeserializeFromStream<T>(Stream stream, params Type[] extraTypes)
            where T: class 
        {
            // Note: Throws System.InvalidOperationException exception "There is an error in XML document (0, 0)." when stream is empty or contains invalid xml.

            XmlSerializer xser = new XmlSerializer(typeof(T), extraTypes);
            //var xser = new DataContractSerializer(typeof(T), extraTypes);
            //var entity = (T)xser.ReadObject(stream);
            var entity = xser.Deserialize(stream) as T;
            return entity;
        }

        public static void SerializeToStream<T>(Stream stream, T entity)
        {
            var xser = new DataContractSerializer(typeof(T));

            xser.WriteObject(stream, entity);
        }
    }
}
