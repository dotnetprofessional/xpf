using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xpf.Scripting;

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
            //Script.SetResourceAssembly(typeof(ScriptTest));
        }


        private const string GetTestTableRecordScript = "Execute_GetTestTableRecord.sql";

        [TestMethod]
        public void Execute_SupportOutParamsOnly()
        {
            string embeddedScriptName = "Execute_SupportOutParams.sql";
            var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .WithOut(new { outParam1 = DbType.Int32 })
                .Execute();

            Assert.AreEqual(1, result.Properties.OutParam1);
        }

        [TestMethod]
        public void Execute_SupportInAndOutParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .WithIn(new {MyParam1 = 2})
                .WithOut(new { outParam1 = DbType.Int32 })
                .Execute();

            Assert.AreEqual(2, result.Properties.OutParam1);
        }

        [TestMethod]
        public void Execute_ScriptNameOnly()
        {
            using (var x = new TransactionScope())
            {
                // Execute an update on the table
                new Script()
                    .Database()
                    .UsingScript("Execute_ScriptNameOnly.sql")
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database()
                    .UsingScript(GetTestTableRecordScript)
                    .WithIn(new { Param1 = 2 })
                    .WithOut(new { outParam1 = DbType.Int32, outParam2 = DbType.AnsiString, outParam3 = DbType.AnsiString})
                    .Execute();


                Assert.AreEqual("Record X", result.Properties.OutParam2);
            }
        }

        [TestMethod]
        public void Execute_WithConnectionString()
        {
            using (var x = new TransactionScope())
            {
                // Execute an update on the table
                new Script()
                    .Database()
                    .WithConnectionString("Data Source=.;Initial Catalog=xpfIOScript;Trusted_Connection=yes;")
                    .UsingScript("Execute_ScriptNameOnly.sql")
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database()
                    .UsingScript(GetTestTableRecordScript)
                    .WithIn(new { Param1 = 2 })
                    .WithOut(new { outParam1 = DbType.Int32, outParam2 = DbType.AnsiString, outParam3 = DbType.AnsiString })
                    .Execute();


                Assert.AreEqual("Record X", result.Properties.OutParam2);
            }
        }

        [TestMethod]
        public void Execute_ScriptNameOnlyWithNestedInclude()
        {
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_IncludeScript.sql";
                new Script()
                    .Database()
                    .UsingScript(embeddedScriptName)
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database()
                    .UsingCommand("SELECT @RowCount = COUNT(*) FROM TestTable WHERE Id >= 10")
                    .WithOut(new { RowCount = DbType.Int32})
                    .Execute();

                Assert.AreEqual(3, result.Properties.RowCount);
            }
        }

        [TestMethod]
        public void Execute_ScriptNameOnlyWithNestedColonR()
        {
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_IncludeScriptUsingColonRSyntax.sql";

                new Script()
                    .Database()
                    .UsingScript(embeddedScriptName)
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database()
                    .UsingCommand("SELECT @RowCount = COUNT(*) FROM TestTable WHERE Id >= 10")
                    .WithOut(new { RowCount = DbType.Int32 })
                    .Execute();

                Assert.AreEqual(3, result.Properties.RowCount);
            }
        }

        [TestMethod]
        public void Execute_DatabaseNameAndScriptName()
        {
            // Essentially the same test as Execute_ScriptNameOnly
            using (var x = new TransactionScope())
            {
                string embeddedScriptName = "Execute_ScriptNameOnly.sql";


                // Execute an update on the table
                new Script()
                    .Database()
                    .UsingScript("Execute_ScriptNameOnly.sql")
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database("xpfScript")
                    .UsingScript(GetTestTableRecordScript)
                    .WithIn(new { Param1 = 2 })
                    .WithOut(new { outParam1 = DbType.Int32, outParam2 = DbType.AnsiString, outParam3 = DbType.AnsiString })
                    .Execute();


                Assert.AreEqual("Record X", result.Properties.OutParam2);
            }
        }

        [TestMethod]
        public void Execute_ScriptNameAndOutParmsAsStringArrayAndInParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Testing the less recommended way of passing outparams as a simple string array
            var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .WithIn(new { MyParam1 = 2 })
                .WithOut(new[] { "outParam1"})
                .Execute();

            Assert.AreEqual(2, result.Properties.OutParam1);
        }


        [TestMethod]
        public void ExecuteReader_ScriptNameOnly()
        {
            string embeddedScriptName = "ExecuteReader_ScriptNameOnly.sql";


            using (var result = new Script()
                .Database("xpfScript")
                .UsingScript(embeddedScriptName)
                .ExecuteReader())
            {
                int count = 0;
                while (result.NextRecord())
                    count++;

                // Now verify results
                Assert.AreEqual(3, count);
            }
        }

        [TestMethod]
        public void ExecuteReader_SupportInParams()
        {
            string embeddedScriptName = "ExecuteReader_SupportInParams.sql";

            using (var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .WithIn(new { Param1 = 2 })
                .ExecuteReader())
            {
                int count = 0;
                while (result.NextRecord())
                    count++;

                // Now verify results
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void ExecuteReader_Access_fields_by_name()
        {
            string embeddedScriptName = "ExecuteReader_SelectAll.sql";

            using (var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .ExecuteReader())
            {
                int count = 0;
                while (result.NextRecord())
                {
                    count ++;
                    Assert.AreEqual("Record " + count, result.Field.Field1);
                    Assert.AreEqual(count, result.Field.Id);
                }

                // Now verify results
                Assert.AreEqual(3, count);
            }
        }

        [TestMethod]
        public void ExecuteReader_FromXmlToInstance()
        {
            string embeddedScriptName = "ExecuteT_ScriptOnly.sql";

            var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .ExecuteReader().FromXmlToInstance<List<TestTable>>();

            Assert.AreEqual(27, result.Count);
        }

        [TestMethod]
        public void ExecuteReader_ToInstance()
        {
            string embeddedScriptName = "ExecuteReader_SelectAll.sql";

            var result = new Script()
                .Database()
                .UsingScript(embeddedScriptName)
                .ExecuteReader().ToInstance<TestTable>();

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void Execute_allows_multiple_scripts_to_be_executed()
        {

            using (var tx = new TransactionScope())
            {
                // Execute the two different scripts
                var actual = new Script()
                    .Database()
                    .UsingScript("Execute_InsertRecord.sql") // Add one new record
                    .WithIn(new {Id = 12})
                    .UsingScript("Execute_InsertRecord.sql") // Add one new record
                    .WithIn(new { Id = 13 })
                    .Execute();

                // Now verify that the update was successful
                var result = new Script()
                    .Database()
                    .UsingCommand("SELECT @RowCount = COUNT(*) FROM TestTable WHERE Id >= 10")
                    .WithOut(new {RowCount = DbType.Int32})
                    .Execute();

                // Verify that the results and properties have been applied correctly
                Assert.AreEqual(2, actual.Results.Count);
                Assert.AreEqual(12, actual.Properties.Id);
                Assert.AreEqual(12, actual.Results[0].Properties.Id);
                Assert.AreEqual(13, actual.Results[1].Properties.Id);

                // Verify that the two scripts wer executed
                Assert.AreEqual(2, result.Properties.RowCount);
            }
        }

        [TestMethod]
        public void Execute_allows_multiple_scripts_to_be_executed_in_parallel()
        {
            try
            {


            // Execute the two different scripts
            var actual = new Script()
                .Database()
                .EnableParallelExecution()
                .UsingScript("Execute_InsertRecord.sql") // Add one new record
                .WithIn(new {Id = 12})
                .UsingScript("Execute_InsertRecord.sql") // Add one new record
                .WithIn(new {Id = 13})
                .Execute();

            // Now verify that the update was successful
            var result = new Script()
                .Database()
                .UsingCommand("SELECT @RowCount = COUNT(*) FROM TestTable WHERE Id >= 10")
                .WithOut(new {RowCount = DbType.Int32})
                .Execute();

            // Verify that the results and properties have been applied correctly
            Assert.AreEqual(2, actual.Results.Count);

            // Unable to verify the properties as the order is no longer guaranteed
            //Assert.AreEqual(12, actual.Properties.Id);
            //Assert.AreEqual(12, actual.Results[0].Properties.Id);
            //Assert.AreEqual(13, actual.Results[1].Properties.Id);

            // Verify that the two scripts wer executed
            Assert.AreEqual(2, result.Properties.RowCount);
            }
            finally
            {
                // This is a compensating script to remove the effects of the previous script
                // a transaction wasn't able to be used due to the use of EnableParallelExecution
                new Script()
                    .Database()
                    .UsingCommand("delete from TestTable where Id > 10")
                    .Execute();
            }
        }
    }
}