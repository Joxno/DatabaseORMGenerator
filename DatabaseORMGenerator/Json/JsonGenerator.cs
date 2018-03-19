using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Json
{
    public class JsonGenerator : IDTOGenerator
    {
        // Privates
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
            var t_ColText = "{ \"Name\":\"" + Column.Name + "\", \"Type\":\"" + _GenerateType(Column.Type) + "\"";
            if (Column.Reference != null)
            {
                var t_ReferenceType = Column.Reference.Type == COLUMN_REFERENCE_TYPE.DESTINATION_TO_SOURCE ? "DTS" : "STD";
                var t_ReferenceRelationship = Column.Reference.Relationship == COLUMN_REFERENCE_RELATIONSHIP.ONE ? "ONE" : "MANY";
                t_ColText += ", \n";
                t_ColText += $"\"Reference\": {{ \"Table\":\"{Column.Reference.Table.Name}\", \"Column\":\"{Column.Reference.Column.Name}\", \"Type\":\"{t_ReferenceType}\",\"Relationship\":\"{t_ReferenceRelationship}\" }}";
            }

            t_ColText += "}";

            return t_ColText;
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
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_File = new ORMSourceFile { Name = Schema.Name + ".json", Content = _GenerateSchema(Schema) };

            return new List<ORMSourceFile> { t_File };
        }
    }
}
