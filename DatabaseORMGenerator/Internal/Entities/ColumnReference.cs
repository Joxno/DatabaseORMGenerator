using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal
{
    public enum COLUMN_REFERENCE_TYPE
    {
        NONE = 0,
        SOURCE_TO_DESTINATION = 1,  // STD S->D Source To Destination
        DESTINATION_TO_SOURCE = 2   // DTS D->S Destination To Source
    }

    public enum COLUMN_REFERENCE_RELATIONSHIP
    {
        NONE = 0,
        ONE = 1,
        MANY = 2
    }

    public class ColumnReference
    {
        public Column Column { get; set; } = null;
        public Table Table { get; set; } = null;
        public COLUMN_REFERENCE_TYPE Type { get; set; } = COLUMN_REFERENCE_TYPE.NONE;
        public COLUMN_REFERENCE_RELATIONSHIP Relationship { get; set; } = COLUMN_REFERENCE_RELATIONSHIP.NONE;
    }
}
