using DatabaseORMGenerator.Cpp.Generators;
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

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_Files = Schema.Tables.Select(T => new CppDTOFileGenerator(T).Generate()).ToList();

            return t_Files;
        }
    }
}
