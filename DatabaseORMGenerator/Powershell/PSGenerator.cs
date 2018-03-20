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

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            return new List<ORMSourceFile> { new ORMSourceFile { Name = Schema.Name + ".ps1", Content = string.Join("\n", Schema.Tables.Select(T => new PSDTOFileGenerator(T).Generate().Content)) } };
        }
    }
}
