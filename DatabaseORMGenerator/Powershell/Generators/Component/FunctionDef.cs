﻿using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators.Component
{
    public class FunctionDef : IFileComponentGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";
        private List<PowershellVariableDef> m_Parameters = new List<PowershellVariableDef>();

        // Interface
        public FunctionDef(string Name)
        {
            m_Name = Name;
        }

        public FunctionDef(string Name, List<PowershellVariableDef> Parameters)
        {
            m_Name = Name;
            m_Parameters = Parameters;
        }

        public FunctionDef(string Name, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Components = Components;
        }

        public FunctionDef(string Name, List<PowershellVariableDef> Parameters, List<IFileComponentGenerator> Components)
        {
            m_Name = Name;
            m_Parameters = Parameters;
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public string Generate()
        {
            var t_Parameters = string.Join(",", m_Parameters.Select(V => V.Generate()));

            var t_Text = $"function {m_Name}({t_Parameters})\n";
            t_Text += "{\n";

            t_Text += string.Join("\n", m_Components.Select(C => C.Generate()));

            t_Text += "\n}\n";

            return t_Text;
        }
    }
}
