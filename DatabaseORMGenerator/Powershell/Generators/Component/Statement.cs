using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class Statement : IFileComponentGenerator
    {
        // Privates
        private string m_Statement = "";

        // Interface
        public Statement(string Statement)
        {
            m_Statement = Statement;
        }
        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            throw new NotImplementedException();
        }

        public string Generate()
        {
            return m_Statement;
        }
    }
}
