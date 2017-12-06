using DatabaseORMGenerator;
using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGeneratorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //var t_Repo = new JsonSchemaRepository(@"C:\Users\Joxno\Documents\Visual Studio 2017\Projects\DatabaseORMGenerator\DatabaseORMGeneratorCLI\bin\Debug\JsonSchema.json");
            var t_Repo = new SqliteSchemaRepository("TOW.db");
            var t_Schema = t_Repo.GetSchema();
            var t_CppGenerator = new CppGenerator();
            var t_SqliteGen = new SqliteGenerator();
            var t_CSharpGen = new CSharpGenerator();
            var t_Files = t_CppGenerator.GenerateSource(t_Schema);
            t_Files.AddRange(t_SqliteGen.GenerateSource(t_Schema));
            t_Files.AddRange(t_CSharpGen.GenerateSource(t_Schema));

            foreach(var t_File in t_Files)
            {
                File.WriteAllText("Generated\\" + t_File.Name, t_File.Content);
            }

            var t_JsonRepo = new JsonSchemaRepository("Generated\\TOW.json");
            t_JsonRepo.SaveSchema(t_Schema);
        }
    }
}
