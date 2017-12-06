using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public class Table
    {
        public string Name { get; set; } = "";
        public Dictionary<int, Column> Columns { get; set; } = new Dictionary<int, Column>();
    }
}
