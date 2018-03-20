using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Internal.Interfaces;
using DatabaseORMGenerator.Powershell.Generators.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators
{
    public class PSContextFileGenerator : IFileGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name;

        private void _SetupComponents(Table Table)
        {
            m_Name = Table.Name;
            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {

            });

            AddComponentGenerator(t_Gen);
        }

        // Interface
        public PSContextFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = m_Name + ".ps1", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
