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
    public class ORMCLIInterface : ICLI
    {
        // Privates
        private ISchemaRepository m_Repo = null;
        private List<IDTOGenerator> m_Generators = new List<IDTOGenerator>();

        private List<GeneratorBinding> m_GeneratorBindings = new List<GeneratorBinding>();

        private CLIOptions m_Options = null;

        private void _ParseArguments()
        {
            if(m_Options.SchemaFrom.EndsWith(".json"))
            {
                m_Repo = new JsonSchemaRepository(m_Options.SchemaFrom);
            }
            else if(m_Options.SchemaFrom.EndsWith(".db"))
            {
                m_Repo = new SqliteSchemaRepository(m_Options.SchemaFrom);
            }

            var t_Generators = m_Options.GenerateDTO.Split(',');
            foreach(var t_GenText in t_Generators)
            {
                var t_Generator = m_GeneratorBindings.Find(B => B.Name == t_GenText);
                if(t_Generator != null)
                {
                    m_Generators.Add((IDTOGenerator)Activator.CreateInstance(t_Generator.GeneratorType));
                }
            }
        }

        // Interface
        public ORMCLIInterface(List<GeneratorBinding> GeneratorBindings)
        {
            m_GeneratorBindings = GeneratorBindings;
        }

        public void Execute()
        {
            var t_Schema = m_Repo.GetSchema();
            var t_DTOFiles = m_Generators.SelectMany(G => G.GenerateSource(t_Schema));

            var t_Path = Path.GetFullPath(m_Options.SaveTo);
            
            foreach(var t_File in t_DTOFiles)
            {
                File.WriteAllText(t_Path + "\\" + t_File.Name, t_File.Content);
            }
        }

        public void SetupArguments(string[] Args)
        {
            var t_Options = new CLIOptions();
            var t_Valid = CommandLine.Parser.Default.ParseArgumentsStrict(Args, t_Options);
            m_Options = t_Options;

            _ParseArguments();
        }
    }
}
