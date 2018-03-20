using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class UDT : IType
    {
        // Private
        private string m_Name = "";
        private string m_Value = "";

        // Interface

        public UDT(string Name, string Value = "")
        {
            m_Name = Name;
            m_Value = Value;
        }

        public string GetDefaultValue()
        {
            return m_Value == "" ? TypeName : m_Value;
        }

        public string GetTypeName()
        {
            return m_Name;
        }

        public string TypeName { get { return GetTypeName(); } set { m_Name = value; } }
        public string DefaultValue { get { return GetDefaultValue(); } set { m_Value = value; } }
    }
}
