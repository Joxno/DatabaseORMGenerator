using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Sqlite.Generators.Component
{
    public class ColumnDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";
        private ColumnType m_Type = null;

        // Interface
        public ColumnDef(string Name, ColumnType Type)
        {
            m_Name = Name;
            m_Type = Type;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            return m_Name + " " + m_Type.GetTypeName();
        }
    }
}
