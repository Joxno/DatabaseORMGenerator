using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class CppGenerator : IDTOGenerator
    {
        // Privates

        private string _GenerateTableSourceFile(Table T)
        {
            var t_File = _GenerateHeader(T.Name);

            foreach (var t_Col in T.Columns.OrderBy((P) => P.Key))
                t_File += "\t" + "\t" + _GenerateTableColumn(t_Col.Value) + ";\n";

            t_File += _GenerateFooter();

            return t_File;
        }

        private string _GenerateTableColumn(Column C)
        {
            return _GenerateType(C.Type) + " " + C.Name;
        }

        private string _GenerateHeader(string Name)
        {
            return 
                "/* AUTOMATICALLY GENERATED CODE */" + '\n' + 
                "#include <string>" + '\n' +
                "class " + Name + '\n' +
                "{" + '\n' +
                "\tpublic:" + '\n' +
                "";
        }

        private string _GenerateFooter()
        {
            return "};";
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
                var t_File = new ORMSourceFile { Name = t_Table.Name + ".h", Content = _GenerateTableSourceFile(t_Table) };
                t_Files.Add(t_File);
            }


            return t_Files;
        }
    }
}
