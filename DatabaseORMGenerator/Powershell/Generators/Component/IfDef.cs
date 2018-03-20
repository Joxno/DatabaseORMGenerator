using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class IfDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_ConditionalBlock = "";

        // Interface
        public IfDef(string ConditionalBlock, List<IFileComponentGenerator> Components)
        {
            m_Components = Components;
            m_ConditionalBlock = ConditionalBlock;
        }
        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return
                $"if({m_ConditionalBlock})" + "\n"
                + "{\n" 
                + string.Join("\n", m_Components.Select(C => C.Generate())) 
                + "\n}\n";
        }
    }
}
