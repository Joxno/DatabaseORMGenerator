using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Sqlite.Generators.Component
{
    public class ColumnType
    {
        // Private
        private COLUMN_DATA_TYPE m_Type = COLUMN_DATA_TYPE.NONE;

        // Interface
        public ColumnType(COLUMN_DATA_TYPE Type)
        {
            m_Type = Type;
        }

        public string GetTypeName()
        {
            var t_TypeText = "";

            if (m_Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "INTEGER";
            if (m_Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "REAL";
            if (m_Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "TEXT";

            return t_TypeText;
        }
    }
}
