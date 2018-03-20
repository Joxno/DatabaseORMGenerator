using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public interface IType
    {
        string GetTypeName();
        string GetDefaultValue();

        string TypeName { get; }
        string DefaultValue { get; }
    }
}
