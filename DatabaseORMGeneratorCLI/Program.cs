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
            // -SchemaFrom
            // -GenerateDTO
            // -GenerateContext
            // -SchemaTo
            // -SaveTo

            var t_CLI = new ORMCLIInterface(
                new List<GeneratorBinding>
                {
                    new GeneratorBinding().Bind<CppGenerator>("C++"),
                    new GeneratorBinding().Bind<CSharpGenerator>("C#"),
                    new GeneratorBinding().Bind<SqliteGenerator>("Sqlite"),
                    new GeneratorBinding().Bind<JSGenerator>("JS")
                }
            );
            t_CLI.SetupArguments(args);
            t_CLI.Execute();
        }
    }
}
