using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public class SqliteSchemaRepository : ISchemaRepository
    {
        // Privates
        private Schema m_Schema = null;

        private Schema _ParseSchema(string FilePath)
        {
            var t_Schema = new Schema();
            using (SQLiteConnection t_Con = new SQLiteConnection($"Data Source=\"{FilePath}\";Version=3;"))
            {
                t_Con.Open();
                using (var t_Reader = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", t_Con).ExecuteReader())
                {
                    while(t_Reader.Read())
                    {
                        var t_TableName = t_Reader.GetString(0);
                        var t_Table = _ParseTable(t_Con, t_TableName);
                        t_Schema.Tables.Add(t_Table);
                    }
                }
            }

            return t_Schema;
        }

        private Table _ParseTable(SQLiteConnection Con, string TableName)
        {
            var t_Table = new Table { Name = TableName };

            using (var t_Reader = new SQLiteCommand($"PRAGMA table_info({TableName});", Con).ExecuteReader())
            {
                while(t_Reader.Read())
                {
                    t_Table.Columns.Add(t_Reader.GetInt32(0), _ParseColumn(t_Reader.GetString(1), t_Reader.GetString(2)));
                }
            }

            return t_Table;
        }

        private Column _ParseColumn(string ColumnName, string ColumnTypeText)
        {
            var t_Col = new Column { Name = ColumnName, Type = _ParseType(ColumnTypeText) };
            return t_Col;
        }

        private COLUMN_DATA_TYPE _ParseType(string TypeText)
        {
            if (TypeText.ToLower().StartsWith("varchar")) TypeText = "varchar";
            if (TypeText.ToLower().StartsWith("nvarchar")) TypeText = "nvarchar";

            switch (TypeText.ToLower())
            {
                case "bit":
                case "bigint":
                case "bool":
                case "int":
                case "integer":
                    return COLUMN_DATA_TYPE.INTEGER;
                case "nvarchar":
                case "varchar":
                case "text":
                    return COLUMN_DATA_TYPE.STRING;
                case "float":
                case "real":
                    return COLUMN_DATA_TYPE.FLOATING;
                default:
                    break;
            }

            return COLUMN_DATA_TYPE.NONE;
        }

        // Interface

        public SqliteSchemaRepository(string SQLiteFilePath)
        {
            m_Schema = _ParseSchema(SQLiteFilePath);
        }

        public Schema GetSchema()
        {
            return m_Schema;
        }
    }
}
