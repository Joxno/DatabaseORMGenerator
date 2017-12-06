using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public class JsonSchemaRepository : ISchemaRepository
    {
        // Privates
        private string m_FilePath = "";
        private Schema m_Schema = null;

        private Schema _ParseFile(string Path)
        {
            var t_Schema = new Schema();

            dynamic t_JsonData = JsonConvert.DeserializeObject(File.ReadAllText(Path));

            foreach(var t_Table in t_JsonData.Tables)
            {
                t_Schema.Tables.Add(_ParseTable(t_Table.Name.ToString(), t_Table.Columns));
            }

            return t_Schema;
        }

        private Table _ParseTable(string Name, dynamic Columns)
        {
            var t_Table = new Table { Name = Name };
            var t_Index = 0;
            foreach(var t_Column in Columns)
            {
                t_Table.Columns.Add(t_Index++, _ParseColumn(t_Column.Name.ToString(), t_Column.Type.ToString()));
            }

            return t_Table;
        }

        private Column _ParseColumn(string Name, string Type)
        {
            var t_Col = new Column { Name = Name };

            switch(Type)
            {
                case "int":
                    t_Col.Type = COLUMN_DATA_TYPE.INTEGER;
                    break;
                case "string":
                    t_Col.Type = COLUMN_DATA_TYPE.STRING;
                    break;
                case "float":
                    t_Col.Type = COLUMN_DATA_TYPE.FLOATING;
                    break;
                default:
                    t_Col.Type = COLUMN_DATA_TYPE.NONE;
                    break;
            }

            return t_Col;
        }

        private string _GenerateSchema(Schema Schema)
        {
            var t_SchemaStr = "{\n";
            t_SchemaStr += "Tables: [ \n";
            var t_TableStr = Schema.Tables.Select(P => _GenerateTable(P)).ToArray();
            t_SchemaStr += string.Join(",\n", t_TableStr);
            t_SchemaStr += "]";
            t_SchemaStr += "\n}";

            return t_SchemaStr;
        }

        private string _GenerateTable(Table Table)
        {
            var t_TableText = "{";

            t_TableText += "\"Name\":\"" + Table.Name + "\", \n";
            t_TableText += "\"Columns\": [\n";

            var t_ColumnsStr = Table.Columns.OrderBy(P => P.Key).Select(P => _GenerateColumn(P.Value)).ToArray();
            t_TableText += string.Join(",\n", t_ColumnsStr);
            t_TableText += "]";
            t_TableText += "}";
            return t_TableText;
        }

        private string _GenerateColumn(Column Column)
        {
            return "{ \"Name\":\"" + Column.Name + "\", \"Type\":\"" + _GenerateType(Column.Type) + "\" }";
        }

        private string _GenerateType(COLUMN_DATA_TYPE Type)
        {
            var t_TypeText = "DEFAULT";

            if (Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "int";
            if (Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "float";
            if (Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "string";

            return t_TypeText;
        }

        // Interface
        public JsonSchemaRepository(string JsonFile)
        {
            m_FilePath = JsonFile;
        }

        public Schema GetSchema()
        {
            m_Schema = _ParseFile(m_FilePath);
            return m_Schema;
        }

        public void SaveSchema(Schema Schema)
        {
            var t_Content = _GenerateSchema(Schema);
            File.WriteAllText(m_FilePath, t_Content);
        }
    }
}
