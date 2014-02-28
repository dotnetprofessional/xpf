using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.Common;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xpf.IO.Test.Models;

namespace xpf.IO.Test
{
    [TestClass]
    public class ScriptTest
    {
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void Setup(TestContext testContext) 
        {
            // Define where the SQL Resource files for these tests exist
            Script.SetResourceAssembly(typeof(ScriptTest));
        }


        private const string GetTestTableRecordScript = "Execute_GetTestTableRecord.sql";

        [TestMethod]
        public void Execute_SupportOutParamsOnly()
        {
            string embeddedScriptName = "Execute_SupportOutParams.sql";

            DbCommand c = Script.Execute(embeddedScriptName, null, new[] { "outParam1" });
            Assert.AreEqual(1, c.Parameters[0].Value);
            Assert.AreEqual(1, c.Parameters["@outParam1"].Value);
        }

        [TestMethod]
        public void Execute_SupportInAndOutParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Pass a single paramter of value 2 to the script
            var c = Script.Execute(embeddedScriptName, new { MyParam1 = 2 }, new[] { "outParam1" });

            Assert.AreEqual(2, c.Parameters[0].Value);
            Assert.AreEqual(2, c.Parameters["@outParam1"].Value);
        }

        [TestMethod]
        public void Execute_ScriptNameOnly()
        {
            using (var x = new TransactionScope())
            {
                DbCommand r = Script.Execute(GetTestTableRecordScript, new { Param1 = 2 }, new[] { "outParam1", "outParam2", "outParam3" });

                // Now verify results
                Assert.AreEqual("Record 2", r.Parameters["@outParam2"].Value);
            }
        }

        [TestMethod]
        public void Execute_ScriptNameOnlyWithNestedInclude()
        {
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_IncludeScript.sql";

                var command = Script.Execute(embeddedScriptName);
                string query = command.CommandText;
            }
        }

        [TestMethod]
        public void Execute_ScriptNameOnlyWithNestedColonR()
        {
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_IncludeScriptUsingColonRSyntax.sql";

                Script.Execute(embeddedScriptName);
            }
        }

        [TestMethod]
        public void Execute_DatabaseNameAndScriptName()
        {
            // Essentially the same test as Execute_ScriptNameOnly
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_ScriptNameOnly.sql";

                // Pass a single paramter of value 2 to the script
                DbCommand c = Script.Execute("xpfIOScript", embeddedScriptName);

                DbCommand r = Script.Execute(GetTestTableRecordScript, new { Param1 = 2 }, new[] { "outParam1", "outParam2", "outParam3" });

                // Now verify results
                Assert.AreEqual("Record X", r.Parameters["@outParam2"].Value);
            }
        }

        [TestMethod]
        public void Execute_ScriptNameAndOutParmsAsStringArrayAndInParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Pass a single paramter of value 2 to the script
            DbCommand c = Script.Execute("default", embeddedScriptName, new { MyParam1 = 2 }, new[] { "outParam1" });

            Assert.AreEqual(2, c.Parameters[0].Value);
            Assert.AreEqual(2, c.Parameters["@outParam1"].Value);
        }

        [TestMethod]
        public void Execute_ScriptNameAndOutParmsAsAnonymouseTypeAndInParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Pass a single paramter of value 2 to the script
            var c = Script.Execute("default", embeddedScriptName, new { MyParam1 = 2 }, new { outParam1 = DbType.Int32 });

            Assert.AreEqual(2, c.Parameters[0].Value);
            Assert.AreEqual(2, c.Parameters["@outParam1"].Value);
        }

        [TestMethod]
        public void Execute_ScriptNameAndNamedOutParmsAndInParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Pass a single paramter of value 2 to the script
            DbCommand c = Script.Execute(embeddedScriptName, new { MyParam1 = 2 }, new[] { "outParam1" });

            Assert.AreEqual(2, c.Parameters[0].Value);
            Assert.AreEqual(2, c.Parameters["@outParam1"].Value);
        }

        [TestMethod]
        public void ExecuteReader_ScriptNameOnly()
        {
            string embeddedScriptName = "ExecuteReader_ScriptNameOnly.sql";

            using (IDataReader dr = Script.ExecuteReader(embeddedScriptName))
            {
                int count = 0;
                while (dr.Read())
                    count++;

                // Now verify results
                Assert.AreEqual(3, count);
            }
        }

        [TestMethod]
        public void ExecuteReader_SupportInParams()
        {
            string embeddedScriptName = "ExecuteReader_SupportInParams.sql";

            // Retrieve the second record
            using (IDataReader dr = Script.ExecuteReader(embeddedScriptName, new { Param1 = 2 }))
            {
                int count = 0;
                while (dr.Read())
                    count++;

                // Now verify results
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void ExecuteReader_SupportInParams_Strings()
        {
            string embeddedScriptName = "ExecuteReader_SupportInParams_Strings.sql";

            // Retrieve the second record
            using (IDataReader dr = Script.ExecuteReader("default", embeddedScriptName, new { Param1 = "Record 2" }))
            {
                int count = 0;
                while (dr.Read())
                    count++;

                // Now verify results
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void ExecuteReaderT_ScriptOnly()
        {
            string embeddedScriptName = "ExecuteT_ScriptOnly.sql";

            // Retrieve the second record
            var result = Script.Execute<List<TestTable>>(embeddedScriptName);
            Assert.AreEqual(27, result.Count);
        }
    }
}
