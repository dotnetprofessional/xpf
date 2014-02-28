using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace xpf.IO
{
    public class Script
    {
        static Assembly resourceAssembly = null;

        public static void SetResourceAssembly(Type typeInAssembly)
        {
            resourceAssembly = Assembly.GetAssembly(typeInAssembly);
        }

        public static void SetResourceAssembly(object instance)
        {
            SetResourceAssembly(instance.GetType());
        }
        /// <summary>
        /// This routine allows easy execution of a script with parameters. The parameter names are defined within the script
        /// such as @myParam. The method takes an anonymous type which maps to the names used with in the script.
        /// </summary>
        /// <param name="databaseName">name of database defined in config file</param>
        /// <param name="embeddedScriptName">name of test script to call</param>
        public static DbCommand Execute(string databaseName, string embeddedScriptName)
        {
            return Execute(databaseName, embeddedScriptName, null, null);
        }

        /// <summary>
        /// This routine allows easy execution of a script with parameters. The parameter names are defined within the script
        /// such as @myParam. The method takes an anonymous type which maps to the names used with in the script.
        /// </summary>
        /// <param name="embeddedScriptName">name of test script to call</param>
        /// <param name="inParameters">an anonymous type defining the paramters and their values eg: new {MyParam1 = 4, MyParam2 = 3}</param>
        /// <param name="outParameters">either a string collection of the out parameter names excluding the @ symbol eg: new [] {"MyReturnValue1", "MyReturnValue2"} or an anonymouse type specifying the name and data type eg: new { MyReturnValue1 = DbType.Int32, MyReturnValue2 = DbType.String }</param>
        /// <returns></returns>
        /// <remarks>
        /// To use this routine you create an SQL script that makes use of parameters defined in the script:
        /// 
        /// @myParam1, @myParam2 etc
        /// 
        /// These parameters can be used in SQL statements such as the following:
        /// SELECT * FROM MyTable Where Field1 = @myParam1
        /// 
        /// To return values from the script you must supply the names as a anonymous type to the ourParameters parameter
        /// 
        /// The values can then be retrieved from the returned DbCommand by the following line assuming c = the result of the method:
        /// 
        /// var value = c.Paramters[0].Value
        /// var value = c.Paramters["@myReturnParam].Value
        /// </remarks>
        public static DbCommand Execute(string embeddedScriptName, object inParameters = null, object outParameters = null)
        {
            return Execute("default", embeddedScriptName, inParameters, outParameters);
        }

        /// <summary>
        /// This routine allows easy execution of a script with parameters and returning an object instance. The parameter names are defined within the script
        /// such as @myParam. The method takes an anonymous type which maps to the names used with in the script.
        /// </summary>
        /// <param name="embeddedScriptName">name of test script to call</param>
        /// <param name="inParameters">an anonymous type defining the paramters and their values eg: new {MyParam1 = 4, MyParam2 = 3}</param>
        /// <returns>instance of T based on XML returned by script</returns>
        /// <remarks>
        /// To use this routine you create an SQL script that makes use of parameters defined in the script:
        /// 
        /// @myParam1, @myParam2 etc
        /// 
        /// These parameters can be used in SQL statements such as the following:
        /// SELECT * FROM MyTable Where Field1 = @myParam1 FOR XML PATH('TestTable'), ROOT('ArrayOfTestTable')
        /// 
        /// To return values from the script you must supply the names as a anonymous type to the ourParameters parameter
        /// 
        /// The values can then be retrieved from the returned DbCommand by the following line assuming c = the result of the method:
        /// 
        /// var value = c.Paramters[0].Value
        /// var value = c.Paramters["@myReturnParam].Value
        /// </remarks>
        public static T Execute<T>(string embeddedScriptName, object inParameters = null)
            where T:class 
        {
            var result = default(T);

            using (var dr = ExecuteReader("default", embeddedScriptName, inParameters))
            {
                var xmlBuilder = new StringBuilder();
                while (dr.Read())
                {
                    xmlBuilder.Append(dr[0]);
                }

                // Now we have the XML we can try desearlizing it.
                if (xmlBuilder.Length > 0)
                    result = Serializer.Deserialize<T>(xmlBuilder.ToString());
            }

            return result;
        }
        /// <summary>
        /// This routine allows easy execution of a script with parameters. The parameter names are defined within the script
        /// such as @myParam. The method takes an anonymous type which maps to the names used with in the script.
        /// </summary>
        /// <param name="databaseName">name of database defined in config file</param>
        /// <param name="embeddedScriptName">name of test script to call</param>
        /// <param name="inParameters">an anonymous type defining the paramters and their values eg: new {MyParam1 = 4, MyParam2 = 3}</param>
        /// <param name="outParameters">either a string collection of the out parameter names excluding the @ symbol eg: new [] {"MyReturnValue1", "MyReturnValue2"} or an anonymouse type specifying the name and data type eg: new { MyReturnValue1 = DbType.Int32, MyReturnValue2 = DbType.String }</param>
        /// <returns></returns>
        /// <remarks>
        /// To use this routine you create an SQL script that makes use of parameters defined in the script:
        /// 
        /// @myParam1, @myParam2 etc
        /// 
        /// These parameters can be used in SQL statements such as the following:
        /// SELECT * FROM MyTable Where Field1 = @myParam1
        /// 
        /// To return values from the script you must supply the names as a anonymous type to the ourParameters parameter
        /// 
        /// The values can then be retrieved from the returned DbCommand by the following line assuming c = the result of the method:
        /// 
        /// var value = c.Paramters[0].Value
        /// var value = c.Paramters["@myReturnParam].Value
        /// </remarks>
        public static DbCommand Execute(string databaseName, string embeddedScriptName, object inParameters, object outParameters)
        {
            var factory = new DatabaseProviderFactory();
            Database dataAccess;

            if (databaseName == "default")
                dataAccess = factory.CreateDefault();
            else
                dataAccess = factory.Create(databaseName);

            DbCommand c;

            string embeddedScript = LoadScript(embeddedScriptName);

            string paramDeclaration = "";

            string executionScript = paramDeclaration + embeddedScript;

            c = dataAccess.GetSqlStringCommand(executionScript);

            if (outParameters != null)
            {
                if (outParameters is string[])
                {
                    foreach (var p in (IEnumerable<string>)outParameters)
                    {
                        dataAccess.AddOutParameter(c, "@" + p, DbType.Object, int.MaxValue);
                    }
                }

                else
                {
                    var properties = outParameters.GetType().GetProperties();
                    foreach (var p in properties)
                    {
                        dataAccess.AddOutParameter(c, "@" + p.Name, (DbType)p.GetValue(outParameters, null), int.MaxValue);
                    }
                }

            }

            if (inParameters != null)
            {
                var properties = inParameters.GetType().GetProperties();
                foreach (var p in properties)
                {
                    dataAccess.AddInParameter(c, "@" + p.Name, ConvertToSqlType(p.PropertyType), p.GetValue(inParameters, null));
                }
            }

            dataAccess.ExecuteNonQuery(c);

            return c;
        }

        private static DbType ConvertToSqlType(Type datatype)
        {
            switch (datatype.Name)
            {
                case "Int32":
                case "Int16":
                    return DbType.Int32;
                case "Int64":
                    return DbType.Int64;
                case "DateTime":
                    return DbType.DateTime;
                case "Guid":
                    return DbType.Guid;
                case "Byte[]":
                    return DbType.Binary;
                default:
                    return DbType.String;
            }
        }

        private static string LoadScript(string embeddedScriptName)
        {
            if(resourceAssembly == null)
                throw new ArgumentException("You must set the Resource Assembly using SetResourceAssembly");

            string embeddedScript = EmbeddedResources.GetResourceString(resourceAssembly, embeddedScriptName);
            // Convert to lines
            var compositeScript = new StringBuilder();
            var scriptLines = embeddedScript.Split(new[] { "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in scriptLines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("include", StringComparison.InvariantCultureIgnoreCase) ||
                    trimmedLine.StartsWith(":r", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Include the embedded script
                    var includeScript = trimmedLine.Substring(trimmedLine.IndexOf(" ", System.StringComparison.Ordinal) + 1);
                    // If the script name is using the :r syntax it might have a sub-path defined, if so need to convert to dot notation
                    includeScript = includeScript.Replace(@"\", ".");

                    compositeScript.Append(LoadScript(includeScript));
                }
                else
                    compositeScript.Append(line + "\r\n");
            }

            return compositeScript.ToString();
        }

        /// <summary>
        /// Used to execute a SQL script (not stored procedure) that has been saved as an embedded resource. 
        /// </summary>
        /// <param name="embeddedScriptName">name of test script to call</param>
        /// <param name="inParameters">an anonymous type defining the paramters and their values eg: new {MyParam1 = 4, MyParam2 = 3}</param>
        public static IDataReader ExecuteReader(string embeddedScriptName, object inParameters = null)
        {
            return ExecuteReader("default", embeddedScriptName, inParameters);
        }

        /// <summary>
        /// Used to execute a SQL script (not stored procedure) that has been saved as an embedded resource. 
        /// </summary>
        /// <param name="databaseName">name of database defined in config file</param>
        /// <param name="embeddedScriptName">name of test script to call</param>
        /// <param name="inParameters">an anonymous type defining the paramters and their values eg: new {MyParam1 = 4, MyParam2 = 3}</param>
        public static IDataReader ExecuteReader(string databaseName, string embeddedScriptName, object inParameters)
        {
            // This is essentially the same code as Execute however it returns a datareader too.
            Database dataAccess;
            var factory = new DatabaseProviderFactory();

            if (databaseName == "default")
                dataAccess = factory.CreateDefault();
            else
                dataAccess = factory.Create(databaseName);

            DbCommand c;

            string embeddedScript = LoadScript(embeddedScriptName);
            string paramDeclaration = "";

            string executionScript = paramDeclaration + embeddedScript;

            c = dataAccess.GetSqlStringCommand(executionScript);

            // Define the input parameters
            if (inParameters != null)
            {
                // Ensure they passed an anonimous type or class
                var properties = inParameters.GetType().GetProperties();
                foreach (var p in properties)
                {
                    dataAccess.AddInParameter(c, "@" + p.Name, ConvertToSqlType(p.PropertyType), p.GetValue(inParameters, null));
                }
            }

            return dataAccess.ExecuteReader(c);
        }
    }
}
