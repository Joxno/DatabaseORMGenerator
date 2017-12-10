using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGeneratorCLI
{
    public class GeneratorBinding
    {
        public string Name { get; set; }
        public Type GeneratorType { get; set; }

        public GeneratorBinding Bind<T>(string Name)
        {
            this.Name = Name;
            GeneratorType = typeof(T);
            return this;
        }
    }
}
