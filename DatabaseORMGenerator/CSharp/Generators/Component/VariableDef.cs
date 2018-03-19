using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.CSharp.Generators.Component
{
    public class VariableDef : IFileComponentGenerator
    {
        // Privates
        private string m_Name = "";
        private TypeDef m_Type = null;

        // Interface
        public VariableDef(string Name, TypeDef Type)
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

        public TypeDef Type { get { return m_Type; } }
    }
}
