using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class ClassDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";
        private List<string> m_Inherits = new List<string>();

        // Interface
        public ClassDef(string Name)
        {
            m_Name = Name;
        }

        public ClassDef(string Name, List<string> Inherits)
        {
            m_Name = Name;
            m_Inherits = Inherits;
        }

        public ClassDef(string Name, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Components = Components;
        }

        public ClassDef(string Name, List<string> Inherits, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Inherits = Inherits;
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return
                $"class {m_Name}" + (m_Inherits.Count > 0 ? " : " + string.Join(",", m_Inherits.Select(S => "public " + S)) : "")
                + "\n{\n"
                + string.Join("\n", m_Components.Select(C => C.Generate()))
                + "\n};";
        }
    }
}
