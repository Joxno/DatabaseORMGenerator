using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class BlockCurly : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private int m_TabCount = 0;

        private string _GenerateTabs()
        {
            var t_Tabs = "";
            for (int i = 0; i < m_TabCount; i++)
                t_Tabs += "\t";
            return t_Tabs;
        }

        // Interface
        public BlockCurly()
        {

        }

        public BlockCurly(int Indentations, List<IFileComponentGenerator> Components)
        {
            m_Components = Components;
            m_TabCount = Indentations;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return "{\n" + string.Join("\n", m_Components.Select(C => _GenerateTabs() + C.Generate())) + "}";
        }
    }
}
