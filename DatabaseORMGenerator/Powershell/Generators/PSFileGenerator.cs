﻿using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseORMGenerator.Internal;

namespace DatabaseORMGenerator.Powershell.Generators
{
    public class PSFileGenerator : IFileGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();

        // Interface
        public PSFileGenerator()
        {

        }

        public PSFileGenerator(List<IFileComponentGenerator> Components)
        {
            m_Components = Components;
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            var t_File = new ORMSourceFile();

            foreach (var t_C in m_Components)
                t_File.Content += t_C.Generate();

            return t_File;
        }
    }
}
