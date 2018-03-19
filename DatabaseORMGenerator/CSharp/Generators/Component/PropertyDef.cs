using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.CSharp.Generators.Component
{
    public class PropertyDef : IFileComponentGenerator
    {
        // Privates
        private VariableDef m_VarDef = null;
        private string m_AccessModifier = "";

        // Interface
        public PropertyDef(VariableDef Variable, string AccessModifier = "public")
        {
            m_VarDef = Variable;
            m_AccessModifier = AccessModifier;
        }

        public PropertyDef(VariableDef Variable, IFileComponentGenerator Get, string AccessModifier = "public")
        {
            m_VarDef = Variable;
            m_AccessModifier = AccessModifier;
        }

        public PropertyDef(VariableDef Variable, IFileComponentGenerator Get, IFileComponentGenerator Set, string AccessModifier = "public")
        {
            m_VarDef = Variable;
            m_AccessModifier = AccessModifier;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {

        }

        public string Generate()
        {
            return $"{m_AccessModifier} {m_VarDef.Generate()} {{get; set;}} = {m_VarDef.Type.GetDefaultValue()};";
        }
    }
}
