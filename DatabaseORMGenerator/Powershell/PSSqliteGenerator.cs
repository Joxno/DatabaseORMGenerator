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
    public class PSSqliteGenerator : IDTOGenerator
    {
        // Privates
        private string _GenerateSchema(Schema Schema)
        {
            return 
                string.Join("", 
                    Schema.Tables
                    .Select(T => new PSRepoFileGenerator(T, "System.Data.SQLite.SQLiteConnection", "System.Data.SQLite.SQLiteCommand")
                    .Generate().Content) 
                ); //string.Join("", Schema.Tables.Select(T => _GenerateRepo(T)));
        }

        private string _GenerateRepo(Table Table)
        {
            var t_File = new MultipleStatement(new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-{Table.Name}DTORepository",
                new List<PowershellVariableDef> { new PowershellVariableDef { DataType = "System.Data.SQLite.SQLiteConnection", Name = "Con" } },
                new List<IFileComponentGenerator>
                {
                    _GenerateRead(Table),
                    new Statement("$_REPO = New-Object -TypeName psobject"),
                    new Statement("$_REPO | Add-Member -MemberType NoteProperty -Name Connection -Value $Con"),
                    new Statement("$_REPO | Add-Member -MemberType ScriptMethod -Name Read -Value $_READ"),
                    new Statement("return $_REPO;")
                })
            });

            return t_File.Generate();
        }

        private string _GenerateCreateFunction(Table Table)
        {
            return "";
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
                        new Statement($"$Reader = [System.Data.SQLite.SQLiteCommand]::new(\"SELECT {t_ColumnsText} FROM {Table.Name};\", $this.Connection).ExecuteReader();"),
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

        private string _GenerateUpdateFunction(Table Table)
        {
            return "";
        }

        private string _GenerateDeleteFunction(Table Table)
        {
            return "";
        }

        private string _GenerateContext(Schema Schema)
        {
            var t_FileGenerator = new MultipleStatement(
            new List<IFileComponentGenerator>
            {
                new FunctionDef($"New-StorageContext", 
                new List<PowershellVariableDef>{ new PowershellVariableDef { DataType = "System.Data.SQLite.SQLiteConnection", Name = "Con" } },
                new List<IFileComponentGenerator>
                {
                    new Statement("$_CONTEXT = New-Object -TypeName psobject"),
                    new Statement("$_CONTEXT | Add-Member -MemberType NoteProperty -Name Connection -Value $Con"),
                    new MultipleStatement
                    (
                        Schema.Tables.Select
                        (
                            T => new Statement($"$_CONTEXT | Add-Member -MemberType NoteProperty -Name {T.Name} -Value $(New-{T.Name}DTORepository($Con))")
                        )
                        .ToList<IFileComponentGenerator>()
                    ),
                    new Statement("return $_CONTEXT;")
                })
            });

            return t_FileGenerator.Generate();
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_PSGen = new PSGenerator();
            var t_Files = t_PSGen.GenerateSource(Schema);
            t_Files.First().Content += _GenerateSchema(Schema) + _GenerateContext(Schema);

            return t_Files;
        }
    }
}
