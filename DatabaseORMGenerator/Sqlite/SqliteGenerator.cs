using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Internal.Interfaces;
using DatabaseORMGenerator.Sqlite.Generators.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class SqliteGenerator : IDTOGenerator
    {
        // Private
        private string _GenerateTable(Table T)
        {
            var t_Gen = new TableDef(T.Name,
                T.Columns
                .OrderBy(P => P.Key)
                .Select(P => new ColumnDef(P.Value.Name, new ColumnType(P.Value.Type)))
                .ToList<IFileComponentGenerator>()
            );

            return t_Gen.Generate();
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            return Schema.Tables.Select(T => new ORMSourceFile { Name = T.Name + ".sql", Content = _GenerateTable(T) }).ToList();
        }
    }
}
