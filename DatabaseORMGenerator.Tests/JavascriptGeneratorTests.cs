using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.Collections.Generic;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class JavascriptGeneratorTests
    {
        [TestMethod]
        public void GenerateJSSource()
        {
            var t_Schema = new Schema
            {
                Name = "TestSchema",
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

            var t_JSGen = new JSGenerator();
            var t_Files = t_JSGen.GenerateSource(t_Schema);

            Assert.IsTrue(t_Files.Count == 1 && t_Files[0].Name == "TestSchema.js" && t_Files[0].Content.Length > 0);
        }
    }
}
