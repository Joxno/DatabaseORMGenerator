using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class SqliteGeneratorTests
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

        private SQLiteConnection _CreateTestDatabase()
        {
            if (File.Exists("TestDatabase.db"))
                File.Delete("TestDatabase.db");

            var t_Con = new SQLiteConnection("Data Source=TestDatabase.db;Version=3;");
            t_Con.Open();

            return t_Con;
        }

        [TestMethod]
        public void GenerateTableFile()
        {
            var t_Schema = _GenerateTestSchema();

            var t_SqliteGen = new SqliteGenerator();
            var t_Files = t_SqliteGen.GenerateSource(t_Schema);


            Assert.IsTrue(t_Files.Count == 1 && t_Files[0].Name == "TestTableSqlite.sql" && t_Files[0].Content.Length > 0);
        }

        [TestMethod]
        public void GenerateDatabaseFromFile()
        {
            var t_Schema = _GenerateTestSchema();

            var t_SqliteGen = new SqliteGenerator();
            var t_Files = t_SqliteGen.GenerateSource(t_Schema);

            var t_Con = _CreateTestDatabase();

            foreach(var t_File in t_Files)
            {
                using (var t_Query = new SQLiteCommand(t_Con))
                {
                    t_Query.CommandText = t_File.Content;

                    t_Query.ExecuteNonQuery();
                }
            }

            string t_TableName = null;
            using (var t_Query = new SQLiteCommand(t_Con))
            {
                t_Query.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='TestTableSqlite';";

                var t_Reader = t_Query.ExecuteReader();
                t_Reader.Read();
                t_TableName = t_Reader.GetString(0); 
            }

            Assert.IsTrue(t_TableName == "TestTableSqlite");
        }
    }
}
