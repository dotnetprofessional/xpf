using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace xpf.Http
{
    internal static class XmlSerializer
    {
        public static T Deserialize<T>(string xml, params Type[] extraTypes)
            where T : class
        {
            T entity;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                entity = DeserializeFromStream<T>(ms, extraTypes);
            }
            return entity;
        }

        public static T DeserializeFromStream<T>(Stream stream, params Type[] extraTypes)
            where T : class
        {
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(T), extraTypes);
            var entity = xser.Deserialize(stream) as T;
            return entity;
        }
        public static void SerializeToStream<T>(Stream stream, T entity)
        {
            var xser = new DataContractSerializer(typeof (T));

            xser.WriteObject(stream, entity);
        }
    }
}
