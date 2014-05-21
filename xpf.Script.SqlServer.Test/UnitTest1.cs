using System;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xpf.Scripting;

namespace xpf.Scripting.SqlServer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var x = new Script()
            //    .Database()
            //    .TakeSnapshot()
            //    .UsingScript("hello.sql")
            //    .WithIn(new {Property1 = "hello"})
            //    .WithOut(new {Property2 = DbType.AnsiString})
            //    .Execute();
        }

        [TestMethod]
        public void Execute_ScriptNameAndNamedOutParmsAndInParams()
        {
            string embeddedScriptName = "Execute_SupportInAndOutParams.sql";

            // Pass a single paramter of value 2 to the script
            //DbCommand c = Script.Execute(embeddedScriptName, new { MyParam1 = 2 }, new[] { "outParam1" });

            var result = new Script()
                .Database()
                .TakeSnapshot()
                .UsingScript(embeddedScriptName)
                .WithIn(new { MyParam1 = 2})
                .WithOut(new { outParam1 = DbType.Int32 })
                .Execute();

            Assert.AreEqual(2, result.Property.OutParam1);
            Assert.AreEqual(2, result.Property.MyParam1);
        }
    }
}
