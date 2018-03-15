using DatabaseORMGenerator.Internal.JsonSchemaInternals;
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
        private List<JsonColumnReference> m_References = null;

        private Schema _ParseFile(string Path)
        {
            m_References = new List<JsonColumnReference>();
            var t_Schema = new Schema();

            dynamic t_JsonData = JsonConvert.DeserializeObject(File.ReadAllText(Path));

            foreach(var t_Table in t_JsonData.Tables)
            {
                t_Schema.Tables.Add(_ParseTable(t_Table.Name.ToString(), t_Table.Columns));
            }

            _ResolveReferences(t_Schema);

            return t_Schema;
        }

        private void _ResolveReferences(Schema Schema)
        {
            foreach (var t_Reference in m_References)
            {
                var t_Table = Schema.Tables.Where(T => T.Name == t_Reference.Table).FirstOrDefault();
                var t_Column = t_Table.Columns.Where(C => C.Value.Name == t_Reference.Column).FirstOrDefault().Value;

                t_Reference.SourceColumn.Reference = new ColumnReference
                {
                    Table = t_Table,
                    Column = t_Column,
                    Type = _ParseReferenceType(t_Reference.Type),
                    Relationship = _ParseReferenceRelationship(t_Reference.Relationship)
                };

                t_Column.Referenced.Add(new ColumnReference
                {
                    Table = t_Reference.SourceTable,
                    Column = t_Reference.SourceColumn,
                    Type = _ParseReferenceType(t_Reference.Type),
                    Relationship = _ParseReferenceRelationship(t_Reference.Relationship)
                });
            }
        }

        private COLUMN_REFERENCE_TYPE _ParseReferenceType(string ReferenceType)
        {
            if (new string[] { "STD", "S->D", "Source To Destination" }.Where(T => T == ReferenceType).FirstOrDefault() != null)
            {
                return COLUMN_REFERENCE_TYPE.SOURCE_TO_DESTINATION;
            }
            else if (new string[] { "DTS", "D->S", "Destination To Source" }.Where(T => T == ReferenceType).FirstOrDefault() != null)
            {
                return COLUMN_REFERENCE_TYPE.DESTINATION_TO_SOURCE;
            }

            return COLUMN_REFERENCE_TYPE.NONE;
        }

        private COLUMN_REFERENCE_RELATIONSHIP _ParseReferenceRelationship(string ReferenceRelationship)
        {
            if (ReferenceRelationship == "ONE")
                return COLUMN_REFERENCE_RELATIONSHIP.ONE;
            else if (ReferenceRelationship == "MANY")
                return COLUMN_REFERENCE_RELATIONSHIP.MANY;

            return COLUMN_REFERENCE_RELATIONSHIP.NONE;
        }

        private Table _ParseTable(string Name, dynamic Columns)
        {
            var t_Table = new Table { Name = Name };
            var t_Index = 0;
            foreach(var t_Column in Columns)
            {
                var t_ParsedColumn = _ParseColumn(
                        t_Column.Name.ToString(),
                        t_Column.Type.ToString(),
                        t_Column.Property?.ToString()
                    );

                if(t_Column.Reference != null)
                {
                    m_References.Add(new JsonColumnReference
                    {
                        SourceTable = t_Table,
                        SourceColumn = t_ParsedColumn,
                        Table = t_Column.Reference.Table.ToString(),
                        Column = t_Column.Reference.Column.ToString(),
                        Type = t_Column.Reference.Type?.ToString(),
                        Relationship = t_Column.Reference.Relationship?.ToString()
                    });
                }

                t_Table.Columns.Add(t_Index++, t_ParsedColumn);
            }

            return t_Table;
        }

        private Column _ParseColumn(string Name, string Type, string Property = null)
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

            switch(Property)
            {
                case "unique":
                    t_Col.Property = COLUMN_PROPERTY_TYPE.UNIQUE;
                    break;
                case "identity":
                    t_Col.Property = COLUMN_PROPERTY_TYPE.IDENTITY;
                    break;
                default:
                    t_Col.Property = COLUMN_PROPERTY_TYPE.NONE;
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
