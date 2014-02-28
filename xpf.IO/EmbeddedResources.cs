using System;
using System.IO;
using System.Reflection;

namespace xpf.IO
{
    /// <summary>
    /// Provides methods to access resource files
    /// </summary>
    public static class EmbeddedResources
    {
        public static Stream GetResourceStream(string resourceName, Type caller)
        {
            var assembly = caller.GetTypeInfo().Assembly;

            var resourceStream = GetResourceStream(resourceName, assembly);
            if (resourceStream == null)
                throw new ArgumentException("Unable to locate resource " + resourceName);
            else
                return resourceStream;
        }

        public static Stream GetResourceStream(string resourceName, Assembly assembly)
        {
            string strFullResourceName = "";
            foreach (string r in assembly.GetManifestResourceNames())
            {
                if (r.EndsWith(resourceName))
                {
                    strFullResourceName = r;
                    break;
                }
            }

            if (strFullResourceName != "")
                return assembly.GetManifestResourceStream(strFullResourceName);
            else
                return null;
        }

        /// <summary>
        /// Returns the contents of an embedded resource file as a string
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName">The name of the resource file to return. The filename is matched using EndsWith allowing for partial filenames to be used.</param>
        /// <returns></returns>
        public static string GetResourceString(Assembly assembly, string resourceName)
        {
            StreamReader objStream;
            string strText = "";

            using (objStream = new StreamReader(GetResourceStream(resourceName, assembly)))
            {
                strText = objStream.ReadToEnd();
            }

            return strText;
        }

        /// <summary>
        /// Returns the contents of an embedded resource file as a string
        /// </summary>
        /// <param name="resourceName">The name of the resource file to return. The filename is matched using EndsWith allowing for partial filenames to be used.</param>
        /// <returns></returns>
        public static byte[] GetResource(Type caller, string resourceName)
        {
            BinaryReader objStream;
            byte[] data;

            using (objStream = new BinaryReader(GetResourceStream(resourceName, caller)))
            {
                data = objStream.ReadBytes((int) objStream.BaseStream.Length);
            }

            return data;
        }


        /// <summary>
        /// Given the name of an embedded xml resource file, will deserialize the file into an instance of T
        /// </summary>
        /// <typeparam name="T">The type that the embedded file represents</typeparam>
        /// <param name="embeddedFile">the name of an embedded resource file. The match uses EndsWith so the full path is not required.</param>
        /// <returns>instance of type T based on the XML of the embedded resource file</returns>
        public static T RetrieveResourceObject<T>(object caller, string embeddedFile, params Type[] extraTypes)
            where T:class 
        {
            return RetrieveResourceObject<T>(caller.GetType().GetTypeInfo().Assembly, embeddedFile, extraTypes);
        }

        public static T RetrieveResourceObject<T>(Assembly assembly, string embeddedFile, params Type[] extraTypes)
            where T:class 
        {
            string xml = GetResourceString(assembly, embeddedFile);
            return Serializer.Deserialize<T>(xml, extraTypes);
        }
    }

}