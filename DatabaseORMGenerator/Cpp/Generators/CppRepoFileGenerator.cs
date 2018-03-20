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
    public class CppRepoFileGenerator : IFileGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
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
                new IncludeUserDef("ISqlContract"),
                new IncludeUserDef(Table.Name + "DTO"),
                new IncludeDef("string"),
                new IncludeDef("vector"),
                new IncludeDef("memory"),
                new IncludeDef("functional"),
                new Block(0, t_References.Concat(t_ReferencedBy).Select(R => new IncludeUserDef(R.Table.Name + "DTO")).ToList<IFileComponentGenerator>()),
                new ClassDef(Table.Name + "DTORepository", new List<IFileComponentGenerator>
                {
                    new ClassPrivateDef(new List<IFileComponentGenerator>
                    {
                        new VariableAssignmentDef(new VariableDef("m_DB", new UDTTemplate("std::shared_ptr", "ISqlContract")), "nullptr"),
                        new Block(1,
                                t_References.Concat(t_ReferencedBy)
                                .Select(R => new VariableAssignmentDef(new VariableDef("m_" + R.Table.Name, new UDTTemplate("std::unique_ptr", R.Table.Name + "DTORepository")), "nullptr"))
                                .ToList<IFileComponentGenerator>()
                            )
                    }),
                    new ClassPublicDef(new List<IFileComponentGenerator>
                    {
                        new ConstructorDef(Table.Name + "DTORepository",new List<VariableDef> { new VariableDef("DB", new UDTTemplate("std::shared_ptr", "ISqlContract")) }, 
                        new List<IFileComponentGenerator>
                        {
                            new AssignmentDef(new VariableDef("m_DB", new UDT("")), "DB"),
                            new Block(1,
                                t_References.Concat(t_ReferencedBy)
                                .Select(R => new VariableAssignmentDef(new VariableDef("m_" + R.Table.Name, new UDTTemplate("std::unique_ptr", R.Table.Name + "DTORepository")), $"std::make_unique<{R.Table.Name}DTORepository>(DB);"))
                                .ToList<IFileComponentGenerator>()
                            )

                        })
                    })
                })
            });

            AddComponentGenerator(t_Gen);
        }

        private FunctionDef _GenerateCreate(Table Table)
        {
            var t_Gen = new FunctionDef("Create", new UDT("void"), new List<IFileComponentGenerator>
            {

            });

            return t_Gen;
        }

        private FunctionDef _GenerateRead(Table Table)
        {
            var t_Gen = new FunctionDef("Read", new UDTTemplate("std::vector", Table.Name + "DTO"), new List<IFileComponentGenerator>
            {

            });

            return t_Gen;
        }

        private FunctionDef _GenerateUpdate(Table Table)
        {
            var t_Gen = new FunctionDef("Update", new UDT("void"), new List<IFileComponentGenerator>
            {

            });

            return t_Gen;
        }

        private FunctionDef _GenerateDelete(Table Table)
        {
            var t_Gen = new FunctionDef("Delete", new UDT("void"), new List<IFileComponentGenerator>
            {

            });

            return t_Gen;
        }

        private Column _GetUniqueColumn(Table Table)
        {
            var t_Unique =
                Table.Columns
                .Where(C => C.Value.Property == COLUMN_PROPERTY_TYPE.UNIQUE)
                .Select(KV => KV.Value).FirstOrDefault();

            return t_Unique;
        }

        // Interface
        public CppRepoFileGenerator(Table Table)
        {
            _SetupComponents(Table);
        }

        public void AddComponentGenerator(IFileComponentGenerator Component)
        {
            m_Components.Add(Component);
        }

        public ORMSourceFile Generate()
        {
            return new ORMSourceFile { Name = m_Name + "DTORepository" + ".h", Content = string.Join("", m_Components.Select(C => C.Generate())) };
        }
    }
}
