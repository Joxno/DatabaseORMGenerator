using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public enum COLUMN_DATA_TYPE
    {
        INTEGER = 0,
        FLOATING = 1,
        STRING = 2,
        NONE = 3
    }

    public class Column
    {
        public string Name { get; set; } = "";
        public COLUMN_DATA_TYPE Type { get; set; } = COLUMN_DATA_TYPE.NONE;
    }
}
