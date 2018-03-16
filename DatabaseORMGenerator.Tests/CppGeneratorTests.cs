using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.Collections.Generic;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class CppGeneratorTests
    {
        [TestMethod]
        public void GenerateCppSource()
        {
            var t_Schema = new Schema
            {
                 Tables = new List<Table>
                 {
                     new Table
                     {
                         Name = "TestTable",
                         Columns = new Dictionary<int, Column>
                         {
                             { 0, new Column { Name = "Id", Type = COLUMN_DATA_TYPE.INTEGER } }
                         }
                     }
                 }
            };

            var t_CppGen = new CppGenerator();
            var t_Files = t_CppGen.GenerateSource(t_Schema);

            Assert.IsTrue(t_Files.Count == 1 && t_Files[0].Name == "TestTableDTO.h" && t_Files[0].Content.Length > 0);
        }
    }
}
