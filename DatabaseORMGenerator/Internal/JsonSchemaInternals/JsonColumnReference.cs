using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Internal.JsonSchemaInternals
{
    public class JsonColumnReference
    {
        public Column SourceColumn { get; set; } = null;
        public Table SourceTable { get; set; } = null;
        public string Table { get; set; } = "";
        public string Column { get; set; } = "";
        public string Type { get; set; } = "";
        public string Relationship { get; set; } = "";
    }
}
