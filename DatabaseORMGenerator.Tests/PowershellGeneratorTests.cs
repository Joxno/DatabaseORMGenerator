using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.Collections.Generic;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class PowershellGeneratorTests
    {
        [TestMethod]
        public void GeneratePowershellDTO()
        {
            var t_Schema = _GenerateTestSchema();

            var t_PSGen = new PSGenerator();
            var t_Files = t_PSGen.GenerateSource(t_Schema);

            Assert.IsTrue(t_Files.Count > 0);
        }

        private Schema _GenerateTestSchema()
        {
            var t_Schema = new Schema
            {
                Tables = new List<Table>
                 {
                     new Table
                     {
                         Name = "TestTablePowershell",
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
