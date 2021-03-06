﻿using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Cpp.Generators.Component
{
    public class VariableAssignmentDef : IFileComponentGenerator
    {
        // Privates
        private VariableDef m_LValue = null;
        private string m_RValue = "";

        // Interface
        public VariableAssignmentDef(VariableDef LValue, string RValue = "")
        {
            m_LValue = LValue;
            m_RValue = RValue;
        }
        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            
        }

        public string Generate()
        {
            return m_LValue.Generate() + " = " + (m_RValue == "" ? (m_LValue.Type.DefaultValue + "{}") : m_RValue) + ";";
        }
    }
}
