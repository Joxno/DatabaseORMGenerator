using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    interface IORMGenerator
    {
        List<ORMSourceFile> GenerateSource(Schema Schema);
    }
}
