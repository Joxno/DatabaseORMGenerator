using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class PowershellVariableDef : IFileComponentGenerator
    {
        public string Name { get; set; } = "";
        public string DataType { get; set; } = "";

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            throw new NotImplementedException();
        }

        public string Generate()
        {
            return $"[{DataType}] ${Name}";
        }
    }
}
