using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseORMGenerator.Internal;

namespace DatabaseORMGenerator.CSharp.Generators
{
    public class CSharpRepositoryFileGenerator : IFileGenerator
    {
        // Private
        List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        
        private void _SetupComponents(Table Table)
        {

        }
        

        // Interface
        public CSharpRepositoryFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = "", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
