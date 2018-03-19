using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.CSharp.Generators.Component;

namespace DatabaseORMGenerator.CSharp.Generators
{
    public class CSharpDTOFileGenerator : IFileGenerator
    {
        // Private
        List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";

        private void _SetupComponents(Table Table)
        {
            m_Name = Table.Name;
            var t_Gen = new MultipleStatement
            (
                new List<IFileComponentGenerator>
                {
                    new Statement("/* AUTOMATICALLY GENERATED CODE */"),
                    new UsingDef("System"),
                    new ClassDef(Table.Name, new List<IFileComponentGenerator>
                    {
                        new MultipleStatement
                        (
                            Table.Columns.OrderBy(P => P.Key).Select(P => new PropertyDef(new VariableDef(P.Value.Name, new TypeDef(P.Value.Type))))
                            .ToList<IFileComponentGenerator>()
                        )
                    })
                }
            );

            AddComponentGenerator(t_Gen);
        }


        // Interface
        public CSharpDTOFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = m_Name + ".cs", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
