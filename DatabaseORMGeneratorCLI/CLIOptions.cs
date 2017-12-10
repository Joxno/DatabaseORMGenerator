using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace DatabaseORMGeneratorCLI
{
    public class CLIOptions
    {
        [Option("SchemaFrom", Required = true, HelpText = "Specifies where to get the Schema from.")]
        public string SchemaFrom { get; set; }

        [Option("GenerateDTO", Required = false, HelpText = "Specifies what languages to generate DTOs for.")]
        public string GenerateDTO { get; set; }

        [Option("SaveTo", HelpText = "Specifies where to save generated files.")]
        public string SaveTo { get; set; }
    }
}
