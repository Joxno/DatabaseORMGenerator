using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public class Schema
    {
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}
