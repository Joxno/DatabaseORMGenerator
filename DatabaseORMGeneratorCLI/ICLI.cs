using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGeneratorCLI
{
    public interface ICLI
    {
        void SetupArguments(string[] Args);
        void Execute();
    }
}
