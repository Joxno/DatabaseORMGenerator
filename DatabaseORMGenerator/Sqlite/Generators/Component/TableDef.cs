using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Sqlite.Generators.Component
{
    public class TableDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";

        // Interface
        public TableDef(string Name)
        {
            m_Name = Name;
        }

        public TableDef(string Name, List<IFileComponentGenerator> Components)
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
            var t_Text = $"CREATE TABLE {m_Name}\n(\n";
            t_Text += string.Join(",\n", m_Components.Select(C => C.Generate()));
            t_Text += ");";

            return t_Text;
        }
    }
}
