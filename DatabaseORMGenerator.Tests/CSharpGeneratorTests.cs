using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DatabaseORMGenerator.Internal;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Linq;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class CSharpGeneratorTests
    {
        [TestMethod]
        public void GenerateCSharpSource()
        {
            var t_Schema = _GenerateTestSchema();
            var t_Repo = new CSharpGenerator();

            var t_Files = t_Repo.GenerateSource(t_Schema);

            Assert.IsTrue(t_Files != null && t_Files.Count > 0);
        }

        [TestMethod]
        public void CheckCompileOfCSharpSource()
        {
            var t_Schema = _GenerateTestSchema();
            var t_Repo = new CSharpGenerator();

            var t_Files = t_Repo.GenerateSource(t_Schema);

            CSharpCodeProvider t_Provider = new CSharpCodeProvider();
            var t_Results = t_Provider.CompileAssemblyFromSource(
                new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true }, 
                t_Files.Select(F => F.Content).ToArray());

            Assert.IsFalse(t_Results.Errors.HasErrors);
            Assert.IsTrue(t_Results.CompiledAssembly.ExportedTypes.Count() > 0);
            Assert.IsTrue(t_Results.CompiledAssembly.ExportedTypes.First().Name == "TestTableCSharp");
            Assert.IsTrue(t_Results.CompiledAssembly.ExportedTypes.First().GetProperties().Count() == 4);
        }

        private Schema _GenerateTestSchema()
        {
            var t_Schema = new Schema
            {
                Tables = new List<Table>
                 {
                     new Table
                     {
                         Name = "TestTableCSharp",
                         Columns = new Dictionary<int, Column>
                         {
                             { 0, new Column { Name = "Id", Type = COLUMN_DATA_TYPE.INTEGER } },
                             { 1, new Column { Name = "Foo", Type = COLUMN_DATA_TYPE.INTEGER } },
                             { 2, new Column { Name = "Bar", Type = COLUMN_DATA_TYPE.INTEGER } },
                             { 3, new Column { Name = "Foobar", Type = COLUMN_DATA_TYPE.INTEGER } }
                         }
                     }
                 }
            };

            return t_Schema;
        }
    }
}
