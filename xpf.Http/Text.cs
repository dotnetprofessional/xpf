using System;
using System.IO;
using System.Runtime.Serialization;

namespace xpf.Http
{
    internal static class XmlSerializer
    {
        public static T DeserializeFromStream<T>(Stream stream, params Type[] extraTypes)
        {
            // Note: Throws System.InvalidOperationException exception "There is an error in XML document (0, 0)." when stream is empty or contains invalid xml.

            //XmlSerializer xser = new XmlSerializer(typeof(T), extraTypes);
            var xser = new DataContractSerializer(typeof (T), extraTypes);
            var entity = (T)xser.ReadObject(stream);
            return entity;

        }

        public static void SerializeToStream<T>(Stream stream, T entity)
        {
            var xser = new DataContractSerializer(typeof (T));

            xser.WriteObject(stream, entity);
        }
    }
}
