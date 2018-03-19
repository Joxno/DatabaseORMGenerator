using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.CSharp.Generators.Component
{
    public class ClassDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";

        // Interface
        public ClassDef(string Name)
        {
            m_Name = Name;
        }

        public ClassDef(string Name, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return 
                $"public class {m_Name}\n{{\n"
                + string.Join("\n", m_Components.Select(C => C.Generate()))
                + "\n}";
        }
    }
}
