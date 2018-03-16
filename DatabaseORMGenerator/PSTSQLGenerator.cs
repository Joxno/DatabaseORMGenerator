using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class PSTSQLGenerator : IDTOGenerator
    {
        // Privates
        public string _GenerateSchema(Schema Schema)
        {
            var t_Content = "";

            foreach (var t_Table in Schema.Tables)
            {

            }

            return t_Content;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_PSGen = new PSGenerator();


            var t_Files = t_PSGen.GenerateSource(Schema);

            t_Files.Add(new ORMSourceFile { Name = Schema.Name + ".ps1", Content = _GenerateSchema(Schema) });

            return t_Files;
        }
    }
}
