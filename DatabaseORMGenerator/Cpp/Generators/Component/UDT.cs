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

        // Interface

        public UDT(string Name, string Value = "")
        {
            TypeName = Name;
            DefaultValue = Value;
        }

        public string GetDefaultValue()
        {
            return DefaultValue;
        }

        public string GetTypeName()
        {
            return TypeName;
        }

        public string TypeName { get; set; } = "";
        public string DefaultValue { get; set; } = "";
    }
}
