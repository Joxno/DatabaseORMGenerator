using DatabaseORMGenerator.Powershell.Generators.Component;
using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Internal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator.Powershell.Generators
{
    public class PSRepoFileGenerator : IFileGenerator
    {
        // Privates
        private List<IFileComponentGenerator> m_Components = new List<IFileComponentGenerator>();
        private string m_Name = "";
        private string m_SqlConnectionType = "";
        private string m_SqlCommandType = "";

        private void _SetupComponents(Table Table)
        {
            m_Name = Table.Name;
            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-{Table.Name}DTORepository",
                new List<PowershellVariableDef> { new PowershellVariableDef { DataType = m_SqlConnectionType, Name = "Con" } },
                new List<IFileComponentGenerator>
                {
                    new Block(1, new List<IFileComponentGenerator>
                    {
                        _GenerateCreate(Table),
                        _GenerateRead(Table),
                        _GetUniqueColumn(Table) == null ? new Statement("### NO UNIQUE COLUMN FOUND FOR UPDATE FUNCTION ###") : _GenerateUpdate(Table),
                        _GetUniqueColumn(Table) == null ? new Statement("### NO UNIQUE COLUMN FOUND FOR DELETE FUNCTION ###") : _GenerateDelete(Table),
                        new Statement("$_REPO = New-Object -TypeName psobject"),
                        new Statement("$_REPO | Add-Member -MemberType NoteProperty -Name Connection -Value $Con"),
                        new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Create -Value $_CREATE"),
                        new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Read -Value $_READ"),
                        _GetUniqueColumn(Table) == null ? new Statement("") : new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Update -Value $_UPDATE"),
                        _GetUniqueColumn(Table) == null ? new Statement("") : new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Delete -Value $_DELETE"),
                        new Statement("return $_REPO;")
                    })
                })
            });

            AddComponentGenerator(t_Gen);
        }

        private IFileComponentGenerator _GenerateCreate(Table Table)
        {
            var t_Columns = Table.Columns.Select(KV => KV.Value);
            var t_ColumnsText = string.Join(",",  t_Columns.Select(K => "[" + K.Name + "]"));
            var t_ColumnValues = string.Join(",", t_Columns.Select(K => K.Type == COLUMN_DATA_TYPE.STRING ? "'$($DTO." + K.Name + ")'" : "$($DTO." + K.Name + ")"));
            var t_ColumnsTextNoIdentity = string.Join(",", t_Columns.Where(C => C.Property != COLUMN_PROPERTY_TYPE.UNIQUE).Select(K => "[" + K.Name + "]")); ;
            var t_ColumnsValuesNoIdentity = string.Join(",", t_Columns.Where(C => C.Property != COLUMN_PROPERTY_TYPE.UNIQUE).Select(K => K.Type == COLUMN_DATA_TYPE.STRING ? "'$($DTO." + K.Name + ")'" : "$($DTO." + K.Name + ")"));
            var t_UniqueColumn = _GetUniqueColumn(Table);

            // var t_InsertQuery = $"SET IDENTITY_INSERT {Table.Name} ON; INSERT INTO {Table.Name}({t_ColumnsText}) VALUES({t_ColumnValues});";
            var t_InsertQuery = $"INSERT INTO {Table.Name}({t_ColumnsText}) VALUES({t_ColumnValues});";
            var t_InsertIdentityQuery = $"INSERT INTO {Table.Name}({t_ColumnsTextNoIdentity}) VALUES({t_ColumnsValuesNoIdentity});";

            var t_Gen = new VariableScriptBlock("_CREATE", new List<IFileComponentGenerator>
            {
                new Block(2, new List<IFileComponentGenerator>
                {
                    new Statement("param($DTO)"),
                    new Statement($"$Query = \"{t_InsertQuery}\";"),
                    new IfDef(t_UniqueColumn != null ? $"$DTO.{t_UniqueColumn.Name} -eq {new TypeDef(t_UniqueColumn.Type).DefaultValue}" : "$false", new List<IFileComponentGenerator>
                    {
                        new Indent(3, new Statement($"$Query = \"{t_InsertIdentityQuery}\";"))
                    }),
                    new Statement($"$Cmd = [{m_SqlCommandType}]::new($Query,$this.Connection);"),
                    new Statement("$Cmd.ExecuteNonQuery();")
                })
            });

            return t_Gen;
        }

        private IFileComponentGenerator _GenerateRead(Table Table)
        {
            var t_Columns = Table.Columns.Select(KV => KV.Value);
            var t_ColumnsText = string.Join(",", t_Columns.Select(K => "[" + K.Name + "]") );

            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new VariableScriptBlock("_READ",
                    new List<IFileComponentGenerator>
                    {
                        new Block(2, new List<IFileComponentGenerator>
                        {
                            new Statement($"$Reader = [{m_SqlCommandType}]::new(\"SELECT {t_ColumnsText} FROM {Table.Name};\", $this.Connection).ExecuteReader();"),
                            new Statement("$DTOs = @();"),
                            new Statement("while ($Reader.Read() -eq $true)"),
                            new ScriptBlock(new List<IFileComponentGenerator>
                            {
                                new Block(3, 
                                    new List<IFileComponentGenerator>
                                    {
                                        new Statement($"$DTO = New-{Table.Name}DTO;")
                                    }.Concat(
                                        t_Columns.Select(C => new Statement($"$DTO.{C.Name} = $Reader[\"{C.Name}\"];")).ToList<IFileComponentGenerator>()
                                    )
                                    .Concat(new List<IFileComponentGenerator>
                                    {
                                        new Statement("$DTOs += $DTO;")
                                    })
                                    .ToList()
                                )
                            }),
                            new Statement("return $DTOs;")
                        })
                    })
            });

            return t_Gen;
        }

        private IFileComponentGenerator _GenerateUpdate(Table Table)
        {
            var t_Unique = _GetUniqueColumn(Table);
            var t_Columns = Table.Columns.Select(KV => KV.Value).Where(C => C != t_Unique);
            var t_ColumnText = string.Join(",", t_Columns.Select(C => $"[{C.Name}] = " + (C.Type == COLUMN_DATA_TYPE.STRING ? $"'$($DTO.{C.Name})'" : "$($DTO.{C.Name})")));
            var t_InsertQuery = $"UPDATE [{Table.Name}] SET {t_ColumnText} WHERE [{t_Unique.Name}] = $($DTO.{t_Unique.Name});";

            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new VariableScriptBlock("_UPDATE", new List<IFileComponentGenerator>
                {
                    new Block(2, new List<IFileComponentGenerator>
                    {
                        new Statement("param($DTO)"),
                        new Statement($"$Cmd = [{m_SqlCommandType}]::new(\"{t_InsertQuery}\",$this.Connection);"),
                        new Statement("$Cmd.ExecuteNonQuery();")
                    })
                })
            });

            return t_Gen;
        }

        private IFileComponentGenerator _GenerateDelete(Table Table)
        {
            var t_Unique = _GetUniqueColumn(Table);
            var t_DeleteQuery = $"DELETE FROM [{Table.Name}] WHERE [{t_Unique.Name}] = " + (t_Unique.Type == COLUMN_DATA_TYPE.STRING ? $"'$($DTO.{t_Unique.Name})'" : $"$($DTO.{t_Unique.Name})") + ";";
            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new VariableScriptBlock("_DELETE", new List<IFileComponentGenerator>
                {
                    new Block(2, new List<IFileComponentGenerator>
                    {
                        new Statement("param($DTO)"),
                        new Statement($"$Cmd = [{m_SqlCommandType}]::new(\"{t_DeleteQuery}\",$this.Connection);"),
                        new Statement("$Cmd.ExecuteNonQuery();")
                    })
                })
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
        public PSRepoFileGenerator(Table Table, string SqlConnectionType, string SqlCommandType)
        {
            m_SqlConnectionType = SqlConnectionType;
            m_SqlCommandType = SqlCommandType;
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
