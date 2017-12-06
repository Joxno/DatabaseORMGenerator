using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class SqliteGenerator : IORMGenerator
    {
        // Private

        private string _GenerateTable(Table T)
        {
            var t_File = "CREATE TABLE " + T.Name + " \n(\n";

            var t_ColumnsStr = T.Columns.OrderBy(P => P.Key).Select(P => _GenerateColumn(P.Value)).ToArray();
            t_File += string.Join(",\n", t_ColumnsStr);

            t_File += "\n)";

            return t_File;
        }

        private string _GenerateColumn(Column C)
        {
            return C.Name + " " + _GenerateType(C.Type);
        }

        private string _GenerateType(COLUMN_DATA_TYPE Type)
        {
            var t_TypeText = "";

            if (Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "INTEGER";
            if (Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "REAL";
            if (Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "TEXT";

            return t_TypeText;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_Files = new List<ORMSourceFile>();

            foreach (var t_Table in Schema.Tables)
                t_Files.Add(new ORMSourceFile { Name = t_Table.Name + ".sql", Content = _GenerateTable(t_Table) });

            return t_Files;
        }
    }
}
