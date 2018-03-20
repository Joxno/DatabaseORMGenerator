using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class AssignmentDef : IFileComponentGenerator
    {
        // Privates
        private VariableDef m_LValue = null;
        private string m_RValue = "";

        // Interface
        public AssignmentDef(VariableDef LValue, string RValue)
        {
            m_LValue = LValue;
            m_RValue = RValue;
        }
        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            
        }

        public string Generate()
        {
            return m_LValue.Name + " = " + m_RValue + ";" + "\n";
        }
    }
}
