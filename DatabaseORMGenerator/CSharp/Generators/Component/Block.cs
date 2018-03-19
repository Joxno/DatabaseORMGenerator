using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.CSharp.Generators.Component
{
    public class Block : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();

        // Interface
        public Block()
        {

        }

        public Block(List<IFileComponentGenerator> Components)
        {
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return "{\n" + string.Join("\n", m_Components.Select(C => C.Generate())) + "}";
        }
    }
}
