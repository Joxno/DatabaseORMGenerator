using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class TypeDef : IType
    {
        private COLUMN_DATA_TYPE m_Type = COLUMN_DATA_TYPE.NONE;

        public string TypeName { get { return GetTypeName(); } }
        public string DefaultValue { get { return GetDefaultValue(); } }

        public TypeDef(COLUMN_DATA_TYPE Type)
        {
            m_Type = Type;
        }

        public string GetTypeName()
        {
            var t_TypeText = "DEFAULT";

            if (m_Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "[int]";
            if (m_Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "[float]";
            if (m_Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "[string]";

            return t_TypeText;
        }

        public string GetDefaultValue()
        {
            var t_TypeText = "DEFAULT";

            if (m_Type == COLUMN_DATA_TYPE.INTEGER) t_TypeText = "-1";
            if (m_Type == COLUMN_DATA_TYPE.FLOATING) t_TypeText = "-1.0f";
            if (m_Type == COLUMN_DATA_TYPE.STRING) t_TypeText = "\"\"";

            return t_TypeText;
        }
    }
}
