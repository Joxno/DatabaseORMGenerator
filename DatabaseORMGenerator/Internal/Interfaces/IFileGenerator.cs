using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal.Interfaces
{
    public interface IFileGenerator
    {
        ORMSourceFile Generate();
        void AddComponentGenerator(IFileComponentGenerator Component);
    }
}
