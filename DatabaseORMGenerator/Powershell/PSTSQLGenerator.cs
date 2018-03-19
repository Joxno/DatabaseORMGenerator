using DatabaseORMGenerator.Internal;
using DatabaseORMGenerator.Internal.Interfaces;
using DatabaseORMGenerator.Powershell.Generators;
using DatabaseORMGenerator.Powershell.Generators.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class PSTSQLGenerator : IDTOGenerator
    {
        // Privates
        private string _GenerateSchema(Schema Schema)
        {
            return string.Join("", Schema.Tables.Select(T => _GenerateRepo(T)));
        }

        private string _GenerateRepo(Table Table)
        {
            var t_File = new PSFileGenerator(new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-{Table.Name}DTORepository", 
                new List<PowershellVariableDef> { new PowershellVariableDef { DataType = "System.Data.SqlClient.SqlConnection", Name = "Con" } },
                new List<IFileComponentGenerator>
                {
                    _GenerateRead(Table),
                    _GenerateCreate(Table),
                    new Statement("$_REPO = New-Object -TypeName psobject"),
                    new Statement("$_REPO | Add-Member -MemberType NoteProperty -Name Connection -Value $Con"),
                    new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Create -Value $_CREATE"),
                    new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Read -Value $_READ"),
                    new Statement("return $_REPO;")
                })
            });

            return t_File.Generate().Content;
        }

        private IFileComponentGenerator _GenerateRead(Table Table)
        {
            var t_Columns = Table.Columns.Select(KV => KV.Value);
            var t_ColumnsText = string.Join(",", t_Columns.Select(K => K.Name));

            var t_Gen = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new VariableScriptBlock("_READ",
                    new List<IFileComponentGenerator>
                    {
                        new Statement($"$Reader = [System.Data.SqlClient.SqlCommand]::new(\"SELECT {t_ColumnsText} FROM {Table.Name};\", $this.Connection).ExecuteReader();"),
                        new Statement("$DTOs = @();"),
                        new Statement("while ($Reader.Read() -eq $true)"),
                        new ScriptBlock(new List<IFileComponentGenerator>
                        {
                            new Statement($"$DTO = New-{Table.Name}DTO;"),
                            new MultipleStatement
                            (
                                t_Columns.Select(C => new Statement($"$DTO.{C.Name} = $Reader[\"{C.Name}\"];")).ToList<IFileComponentGenerator>()
                            ),
                            new Statement("$DTOs += $DTO;")
                        }),
                        new Statement("return $DTOs;")
                    })
            });
            
            return t_Gen;
        }

        private IFileComponentGenerator _GenerateCreate(Table Table)
        {
            var t_Columns = Table.Columns.Select(KV => KV.Value);
            var t_ColumnsText = string.Join(",", t_Columns.Select(K => K.Name));
            var t_ColumnValues = string.Join(",", t_Columns.Select(K => "'$($DTO." + K.Name + ")'"));
            var t_InsertQuery = $"SET IDENTITY_INSERT {Table.Name} ON; INSERT INTO {Table.Name}({t_ColumnsText}) VALUES({t_ColumnValues});";

            var t_Gen = new VariableScriptBlock("_CREATE", new List<IFileComponentGenerator>
            {
                new Statement("param($DTO)"),
                new Statement($"$Cmd = [System.Data.SqlClient.SqlCommand]::new(\"{t_InsertQuery}\",$this.Connection);"),
                new Statement("$Cmd.ExecuteNonQuery();")
            });

            return t_Gen;
        }


        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_PSGen = new PSGenerator();
            var t_Files = t_PSGen.GenerateSource(Schema);

            t_Files.First().Content += _GenerateSchema(Schema); // + _GenerateContext(Schema);

            return t_Files;
        }
    }
}
