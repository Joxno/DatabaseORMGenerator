using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DatabaseORMGenerator.Internal
{
    public class TSQLSchemaRepository : ISchemaRepository
    {
        // Privates
        private string m_ConnectionString = "";
        private Schema m_Schema = null;

        private Schema _ParseSchema(string ConnectionString)
        {
            ConnectionString = _ForceMultipleSets(ConnectionString);

            var t_Schema = new Schema();
            using (var t_Con = new SqlConnection(ConnectionString))
            {
                t_Con.Open();

                using (var t_Reader = new SqlCommand("SELECT name, object_id FROM sys.tables WHERE type = 'U';", t_Con).ExecuteReader())
                {
                    while(t_Reader.Read())
                    {
                        var t_Table = _ParseTable(t_Con, t_Reader.GetString(0), t_Reader.GetInt32(1));
                        t_Schema.Tables.Add(t_Table);
                    }
                }
            }

            return t_Schema;
        }

        private string _ForceMultipleSets(string ConnectionString)
        {
            if(!ConnectionString.Contains("MultipleActiveResultSets"))
            {
                if (ConnectionString.Last() != ';')
                    ConnectionString += ';';
                ConnectionString += "MultipleActiveResultSets=True;";
            }

            return ConnectionString;
        }

        private Table _ParseTable(SqlConnection Con, string TableName, int TableObjectId)
        {
            var t_Table = new Table { Name = TableName };
            var t_Query = @"
SELECT tabl.name AS TableName, C.column_id, C.name AS ColumnName, T.name AS TypeName, C.is_identity, indx.column_id, indx.is_primary_key, CAST(indx.is_unique AS int) AS is_unique, indx.name
FROM sys.tables Tabl
INNER JOIN sys.all_columns C ON C.object_id = Tabl.object_id
INNER JOIN sys.types T ON T.user_type_id = C.user_type_id
OUTER APPLY
(
	SELECT IC.column_id, I.is_primary_key, I.is_unique, I.name
	FROM sys.index_columns IC
	INNER JOIN sys.indexes I ON I.index_id = IC.index_id AND I.object_id = IC.object_id
	WHERE IC.object_id = tabl.object_id AND IC.column_id = C.column_id
) AS indx
WHERE type = 'U' AND tabl.object_id = [_OBJECT_ID_]"
.Replace("[_OBJECT_ID_]", TableObjectId.ToString());

            using (var t_Reader = new SqlCommand(t_Query, Con).ExecuteReader())
            {
                while(t_Reader.Read())
                {
                    var t_Column = _ParseColumn
                        (
                            t_Reader["ColumnName"].ToString(),
                            t_Reader["TypeName"].ToString(),
                            t_Reader.IsDBNull(t_Reader.GetOrdinal("is_unique")) ? false : int.Parse(t_Reader["is_unique"].ToString()) == 1 ? true : false
                        );
                    t_Table.Columns.Add(t_Reader.GetInt32(t_Reader.GetOrdinal("column_id")) - 1, t_Column);
                }
            }

            return t_Table;
        }

        private Column _ParseColumn(string ColumnName, string ColumnTypeText, bool IsUnique)
        {
            var t_Col = new Column
            {
                Name = ColumnName,
                Type = _ParseType(ColumnTypeText),
                Property = IsUnique ? COLUMN_PROPERTY_TYPE.UNIQUE : COLUMN_PROPERTY_TYPE.NONE
            };

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
                case "char":
                case "nvarchar":
                case "varchar":
                case "text":
                    return COLUMN_DATA_TYPE.STRING;
                case "float":
                case "double":
                case "numerical":
                case "decimal":
                case "real":
                    return COLUMN_DATA_TYPE.FLOATING;
                default:
                    break;
            }

            return COLUMN_DATA_TYPE.NONE;
        }

        // Interface

        public TSQLSchemaRepository(string SQLConnectionString)
        {
            m_ConnectionString = SQLConnectionString;
        }

        public Schema GetSchema()
        {
            m_Schema = _ParseSchema(m_ConnectionString);
            return m_Schema;
        }

        public void SaveSchema(Schema Schema)
        {
            throw new NotImplementedException();
        }
    }
}
