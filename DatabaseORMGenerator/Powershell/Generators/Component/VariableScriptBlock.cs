using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class VariableScriptBlock : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_VariableName = "";

        // Interface
        public VariableScriptBlock(string VariableName, List<IFileComponentGenerator> Components)
        {
            m_VariableName = VariableName;
            m_Components = Components;
        }
        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return $"${m_VariableName} = {{\n" + string.Join("\n", m_Components.Select(C => C.Generate())) + "\n}\n";
        }
    }
}
