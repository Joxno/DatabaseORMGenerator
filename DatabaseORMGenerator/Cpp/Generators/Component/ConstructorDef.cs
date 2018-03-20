using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class ConstructorDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";
        private List<VariableDef> m_Parameters = new List<VariableDef>();

        // Interface
        public ConstructorDef(string Name)
        {
            m_Name = Name;
        }

        public ConstructorDef(string Name, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Components = Components;
        }

        public ConstructorDef(string Name, List<VariableDef> Parameters, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Parameters = Parameters;
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            var t_ParameterText = string.Join(",", m_Parameters.Select(P => P.Generate()));
            return $"{m_Name}({t_ParameterText})" + "\n"
                + "{" + "\n"
                + string.Join("\n", m_Components.Select(C => C.Generate()))
                + "}" + "\n";
        }
    }
}
