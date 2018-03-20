using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Powershell.Generators.Component;

namespace DatabaseORMGenerator.Powershell.Generators
{
    public class PSDTOFileGenerator : IFileGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name;

        private void _SetupComponents(Table Table)
        {
            m_Name = Table.Name;
            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-{Table.Name}DTO", new List<IFileComponentGenerator>
                {
                    new MultipleStatement(new List<IFileComponentGenerator>
                    {
                        new Block(1,
                            new List<IFileComponentGenerator>
                            {
                                new Statement("$_OBJ = New-Object -TypeName PSObject")
                            }
                            .Concat(
                                Table.Columns.Select(KV => new Statement($"$_OBJ | Add-Member -MemberType NoteProperty -Name {KV.Value.Name} -Value {new TypeDef(KV.Value.Type).DefaultValue}"))
                            )
                            .Concat(new List<IFileComponentGenerator>
                            {
                                new Statement("return $_OBJ;")
                            }).ToList()
                        )
                    })
                })
            });

            AddComponentGenerator(t_Gen);
        }

        // Interface
        public PSDTOFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = m_Name + ".ps1", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
