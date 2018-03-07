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

    public enum COLUMN_KEY_TYPE
    {
        NONE = 0,
        PRIMARY = 1
    }

    public enum COLUMN_PROPERTY_TYPE
    {
        NONE = 0,
        UNIQUE = 1,
        IDENTITY = 2
    }

    public class Column
    {
        public string Name { get; set; } = "";
        public COLUMN_DATA_TYPE Type { get; set; } = COLUMN_DATA_TYPE.NONE;
        public COLUMN_KEY_TYPE Key { get; set; } = COLUMN_KEY_TYPE.NONE;
        public COLUMN_PROPERTY_TYPE Property { get; set; } = COLUMN_PROPERTY_TYPE.NONE;

        public ColumnReference Reference { get; set; } = null;
    }
}
