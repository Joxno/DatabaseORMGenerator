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
            var t_Variables = "";
            var t_Includes = 
                "#include <string>" + '\n' +
                "#include <vector>" + '\n';

            foreach (var t_Col in T.Columns.OrderBy((P) => P.Key))
                t_Variables += "\t" + "\t" + _GenerateTableColumn(t_Col.Value) + ";\n";

            //var t_References = T.Columns.Where(C => C.Value.Reference != null).Select(KV => KV.Value.Reference);
            //foreach(var t_Reference in t_References)
            //{
            //    t_Variables += "\t" + "\t" + $"{t_Reference.Table.Name}DTO m_{t_Reference.Table.Name};\n";
            //    t_Includes += $"#include \"{t_Reference.Table.Name}DTO.h\"" + '\n';
            //}

            var t_ReferencedBy = T.Columns.Where(C => C.Value.Referenced.Count > 0).SelectMany(KV => KV.Value.Referenced);
            foreach (var t_Referenced in t_ReferencedBy)
            {
                t_Variables += "\t" + "\t" + $"std::vector<{t_Referenced.Table.Name}DTO> {t_Referenced.Table.Name};\n";
                t_Includes += $"#include \"{t_Referenced.Table.Name}DTO.h\"" + '\n';
            }

            var t_Header = "/* COMPUTER GENERATED CODE */" + '\n' +
                "#pragma once" + '\n' +
                t_Includes +
                $"class {T.Name}DTO" + '\n' +
                "{" + '\n' +
                "\tpublic:" + '\n' +
                "";

            var t_Footer = "};";

            return t_Header + t_Variables + t_Footer;
        }

        private string _GenerateTableColumn(Column C)
        {
            return _GenerateType(C.Type) + " " + C.Name;
        }


        private string _GenerateType(COLUMN_DATA_TYPE Type)
        {
            var t_TypeText = "DEFAULT";

            if (Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "int";
            if (Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "float";
            if (Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "std::string";

            return t_TypeText;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_Files = new List<ORMSourceFile>();

            foreach(var t_Table in Schema.Tables)
            {
                var t_File = new ORMSourceFile { Name = t_Table.Name + "DTO.h", Content = _GenerateTableSourceFile(t_Table) };
                t_Files.Add(t_File);
            }


            return t_Files;
        }
    }
}
