using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class PSGenerator : IDTOGenerator
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
            var t_Columns = string.Join(", ", Table.Columns.Select(C => "$" + C.Value.Name));
            var t_Content = "function New-" + Table.Name + "DTO() \n{\n";

            t_Content += "\t$_OBJ = New-Object -TypeName PSObject \n";
            foreach (var t_Column in Table.Columns)
            {
                t_Content += $"\t$_OBJ | Add-Member -MemberType NoteProperty -Name {t_Column.Value.Name} -Value ''" + '\n';
            }

            t_Content += "\n\treturn $_OBJ;" + '\n';

            t_Content += "}\n";

            return t_Content;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_Files = new List<ORMSourceFile>();

            t_Files.Add(new ORMSourceFile { Name = Schema.Name + ".ps1", Content = _GenerateSchema(Schema) });

            return t_Files;
        }
    }
}
