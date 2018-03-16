using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class SqliteSchemaRepositoryTests
    {
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

        private void _CreateTestDatabase(Schema Schema)
        {
            if(System.IO.File.Exists("SqliteTestDatabase.db"))
                System.IO.File.Delete("SqliteTestDatabase.db");

            var t_SqliteGenerator = new SqliteGenerator();
            var t_Files = t_SqliteGenerator.GenerateSource(Schema);

            using (var t_Con = new SQLiteConnection("Data Source=SqliteTestDatabase.db;Version=3;").OpenAndReturn())
            {
                foreach(var t_File in t_Files)
                {
                    new SQLiteCommand(t_File.Content, t_Con).ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void GenerateSchemaFromDatabase()
        {
            _CreateTestDatabase(_GenerateTestSchema());

            var t_SqliteRepo = new SqliteSchemaRepository("SqliteTestDatabase.db");
            var t_Schema = t_SqliteRepo.GetSchema();

            Assert.IsTrue(t_Schema != null && t_Schema.Tables.Count > 0 && t_Schema.Tables[0].Name == "TestTableSqlite");
        }
    }
}
