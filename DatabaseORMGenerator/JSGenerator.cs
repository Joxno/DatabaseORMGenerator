using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class JSGenerator : IDTOGenerator
    {
        // Privates

        public string _GenerateSchema(Schema Schema)
        {
            var t_Content = "";

            foreach (var t_Table in Schema.Tables)
                t_Content += _GenerateTable(t_Table) + "\n";

            return t_Content;
        }

        public string _GenerateTable(Table Table)
        {
            var t_Content = "function " + Table.Name + " {\n";

            foreach (var t_Column in Table.Columns)
                t_Content += _GenerateColumn(t_Column.Value) + "\n";

            t_Content += "}";

            return t_Content;
        }

        public string _GenerateColumn(Column Column)
        {
            var t_Content = "";

            t_Content = "this." + Column.Name + " = " + (Column.Type == COLUMN_DATA_TYPE.STRING ? "\"\"" : "-1") + ";";

            return t_Content;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_Files = new List<ORMSourceFile>();

            t_Files.Add(new ORMSourceFile { Name = Schema.Name + ".js", Content = _GenerateSchema(Schema) });

            return t_Files;
        }
    }
}
