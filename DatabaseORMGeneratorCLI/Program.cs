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
            args = new string[] { "--SchemaFrom", "TOW.json", "--GenerateDTO", "PSSqlite", "--SaveTo", "Generated" };
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
                    new GeneratorBinding().Bind<JSGenerator>("JS"),
                    new GeneratorBinding().Bind<CppSqlGenerator>("C++SQL"),
                    new GeneratorBinding().Bind<PSGenerator>("PS"),
                    new GeneratorBinding().Bind<PSTSQLGenerator>("PSTSQL"),
                    new GeneratorBinding().Bind<PSSqliteGenerator>("PSSqlite")
                }
            );
            t_CLI.SetupArguments(args);
            t_CLI.Execute();
        }
    }
}
