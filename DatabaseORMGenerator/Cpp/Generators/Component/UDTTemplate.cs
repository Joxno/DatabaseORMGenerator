using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class UDTTemplate : IType
    {

        // Private
        private string m_TemplatedName = "";

        // Interface

        public UDTTemplate(string Name, string TemplatedName, string Value = "")
        {
            TypeName = Name;
            DefaultValue = Value;
            m_TemplatedName = TemplatedName;
        }

        public string GetDefaultValue()
        {
            return DefaultValue == "" ? GetTypeName() : DefaultValue;
        }

        public string GetTypeName()
        {
            return TypeName + $"<{m_TemplatedName}>";
        }

        public string TypeName { get; set; } = "";
        public string DefaultValue { get; set; } = "";
    }
}
