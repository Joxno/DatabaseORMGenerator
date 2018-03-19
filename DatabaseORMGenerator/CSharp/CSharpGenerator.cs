using DatabaseORMGenerator.CSharp.Generators;
using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class CSharpGenerator : IDTOGenerator
    {
        // Privates

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            return Schema.Tables.Select(T => new CSharpDTOFileGenerator(T).Generate()).ToList();
        }
    }
}
