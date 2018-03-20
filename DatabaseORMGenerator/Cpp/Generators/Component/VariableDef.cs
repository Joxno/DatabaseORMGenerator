using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class VariableDef : IFileComponentGenerator
    {
        // Privates
        private string m_Name = "";
        private IType m_Type = null;

        // Interface
        public VariableDef(string Name, IType Type)
        {
            m_Name = Name;
            m_Type = Type;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {

        }

        public string Generate()
        {
            return $"{m_Type.GetTypeName()} {m_Name}";
        }

        public IType Type { get { return m_Type; } }
        public string Name { get { return m_Name; } }
    }
}
