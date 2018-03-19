using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Internal.Interfaces;
using DatabaseORMGenerator.Powershell.Generators;
using DatabaseORMGenerator.Powershell.Generators.Component;
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
        private string _GenerateSchema(Schema Schema)
        {
            return string.Join("\n", Schema.Tables.Select(T => _GenerateTable(T)));
        }

        private string _GenerateTable(Table Table)
        {
            var t_FileGenerator = new PSFileGenerator(new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-{Table.Name}DTO", new List<IFileComponentGenerator>
                {
                    new Statement("$_OBJ = New-Object -TypeName PSObject"),
                    new MultipleStatement
                    (
                        Table.Columns.Select(KV => new Statement($"$_OBJ | Add-Member -MemberType NoteProperty -Name {KV.Value.Name} -Value ''"))
                        .ToList<IFileComponentGenerator>()
                    ),
                    new Statement("return $_OBJ;")
                })
            });

            return t_FileGenerator.Generate().Content;
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
