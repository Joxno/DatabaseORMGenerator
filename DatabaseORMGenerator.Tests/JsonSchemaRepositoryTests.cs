using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.IO;
using System.Collections.Generic;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class JsonSchemaRepositoryTests
    {
        [TestMethod]
        public void GetSchemaFromJsonFile()
        {
            File.WriteAllText("JsonTestSchema.json", _CreateJsonFile());
            var t_Repo = new JsonSchemaRepository("JsonTestSchema.json");
            var t_Schema = t_Repo.GetSchema();

            Assert.IsTrue(t_Schema != null && t_Schema.Tables.Count > 0 && t_Schema.Tables[0].Name == "TestFoobar");
        }

        [TestMethod]
        public void SaveSchemaToJsonFile()
        {
            var t_Schema = _GenerateTestSchema();

            var t_Repo = new JsonSchemaRepository("SaveJsonTestSchema.json");
            t_Repo.SaveSchema(t_Schema);

            Assert.IsTrue(File.Exists("SaveJsonTestSchema.json"));

            var t_InterpretedSchema = t_Repo.GetSchema();

            Assert.IsTrue(t_InterpretedSchema != null && t_InterpretedSchema.Tables.Count > 0);
        }

        private string _CreateJsonFile()
        {
            var t_Str =
            @"{
    Tables: [
        {
            Name : ""TestFoobar"",
            Columns: [
                { Name:""Id"", Type:""int"" },
                { Name:""Foobar"", Type:""string"" },
            ]
        }
    ]
}";

            return t_Str;
        }

        private Schema _GenerateTestSchema()
        {
            var t_Schema = new Schema
            {
                Tables = new List<Table>
                 {
                     new Table
                     {
                         Name = "TestTableSqlite",
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
