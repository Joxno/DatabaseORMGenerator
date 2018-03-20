using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Cpp.Generators.Component;

namespace DatabaseORMGenerator.Cpp.Generators
{
    public class CppDTOFileGenerator : IFileGenerator
    {
        // Private
        List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";

        private void _SetupComponents(Table Table)
        {
            var t_References = Table.Columns.Where(KV => KV.Value.Reference != null).Select(KV => KV.Value.Reference).Where(R => R.Type == COLUMN_REFERENCE_TYPE.SOURCE_TO_DESTINATION);
            var t_ReferencedBy = Table.Columns.Where(KV => KV.Value.Referenced.Count > 0).SelectMany(KV => KV.Value.Referenced).Where(R => R.Type == COLUMN_REFERENCE_TYPE.DESTINATION_TO_SOURCE);
            m_Name = Table.Name;
            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new Statement("/* COMPUTER GENERATED CODE */"),
                new Statement("#pragma once"),
                new IncludeDef("string"),
                new IncludeDef("vector"),
                new Block(0, t_References.Concat(t_ReferencedBy).Select(R => new IncludeUserDef(R.Table.Name + "DTO")).ToList<IFileComponentGenerator>()),
                new ClassDef(Table.Name + "DTO", new List<IFileComponentGenerator>
                {
                    new ClassPublicDef
                    (
                        new List<IFileComponentGenerator>
                        {
                            new Block(1,
                                Table.Columns
                                .OrderBy(C => C.Key)
                                .Select(C => new VariableAssignmentDef
                                (
                                    new VariableDef(C.Value.Name, new TypeDef(C.Value.Type)),
                                    new TypeDef(C.Value.Type).DefaultValue
                                ))
                                .ToList<IFileComponentGenerator>()
                            ),
                            new Block(1,
                                t_References.Concat(t_ReferencedBy)
                                .Where(R => R.Relationship == COLUMN_REFERENCE_RELATIONSHIP.ONE)
                                .Select(R => new VariableAssignmentDef(new VariableDef(R.Table.Name, new UDT(R.Table.Name + "DTO"))))
                                .ToList<IFileComponentGenerator>()
                            ),
                            new Block(1,
                                t_References.Concat(t_ReferencedBy)
                                .Where(R => R.Relationship == COLUMN_REFERENCE_RELATIONSHIP.MANY)
                                .Select(R => new VariableAssignmentDef(new VariableDef(R.Table.Name, new UDT($"std::vector<{R.Table.Name + "DTO"}>"))))
                                .ToList<IFileComponentGenerator>()
                            )
                        }
                        
                    )
                })
            });

            AddComponentGenerator(t_Gen);
        }

        // Interface
        public CppDTOFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = m_Name + "DTO" + ".h", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
