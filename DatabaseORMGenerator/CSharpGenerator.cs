using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class CSharpGenerator : IORMGenerator
    {
        // Privates

        private string _GenerateTable(Table Table)
        {
            var t_Includes = "using System;\n";
            var t_Content = t_Includes + $"public class {Table.Name}\n";
            t_Content += "{\n";
            t_Content += string.Join("\n", Table.Columns.OrderBy(P => P.Key).Select(P => _GenerateColumn(P.Value)).ToArray());
            t_Content += "\n}";
            return t_Content;
        }

        private string _GenerateColumn(Column Column)
        {
            return "public " + _GenerateType(Column.Type) + " " + Column.Name + " { get; set; }";
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
            var t_Files = new List<ORMSourceFile>();

            foreach(var t_Table in Schema.Tables)
            {
                t_Files.Add(new ORMSourceFile { Name = t_Table.Name + ".cs", Content = _GenerateTable(t_Table) });
            }


            return t_Files;
        }
    }
}
