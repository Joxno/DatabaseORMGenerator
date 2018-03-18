using DatabaseORMGenerator.Internal;
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
            var t_Content = "";

            foreach (var t_Table in Schema.Tables)
            {
                t_Content += _GenerateRepo(t_Table);
            }

            return t_Content;
        }

        private string _GenerateRepo(Table Table)
        {
            var t_Text =
@"
function New-[TABLE_NAME]DTORepository([System.Data.SQLite.SQLiteConnection] $Con)
{
    $CreateFunc = {
    [CREATE_FUNCTION]    
    }
    $ReadFunc = {
    [READ_FUNCTION]
    }
    $UpdateFunc = {
    [UPDATE_FUNCTION]    
    }
    $DeleteFunc = {
    [DELETE_FUNCTION]
    }
    
    $_REPO = New-Object -Type psobject
    $_REPO | Add-Member -MemberType NoteProperty -Name Connection -Value $Con
    $_REPO | Add-Member -MemberType ScriptMethod -Name Create -Value $CreateFunc
    $_REPO | Add-Member -MemberType ScriptMethod -Name Read -Value $ReadFunc
    $_REPO | Add-Member -MemberType ScriptMethod -Name Update -Value $UpdateFunc
    $_REPO | Add-Member -MemberType ScriptMethod -Name Delete -Value $DeleteFunc

    return $_REPO;
}"
.Replace("[TABLE_NAME]", Table.Name)
.Replace("[CREATE_FUNCTION]", _GenerateCreateFunction(Table))
.Replace("[READ_FUNCTION]", _GenerateReadFunction(Table))
.Replace("[UPDATE_FUNCTION]", _GenerateUpdateFunction(Table))
.Replace("[DELETE_FUNCTION]", _GenerateDeleteFunction(Table))
+'\n';

            return t_Text;
        }

        private string _GenerateCreateFunction(Table Table)
        {
            return "";
        }

        private string _GenerateReadFunction(Table Table)
        {
            var t_Columns = Table.Columns.Select(KV => KV.Value);
            var t_ColumnsText = string.Join(",", t_Columns.Select(K => K.Name));
            var t_AssignmentText = "";
            foreach(var t_C in t_Columns)
            {
                t_AssignmentText += $"$DTO.{t_C.Name} = $Reader[\"{t_C.Name}\"];" + '\n';
            }

            var t_Text = $@"$Reader = [System.Data.SQLite.SQLiteCommand]::new(""SELECT {t_ColumnsText} FROM {Table.Name};"", $this.Connection).ExecuteReader();
    $DTOs = @();
    while ($Reader.Read() -eq $true)
    {{
    $DTO = New-{Table.Name}DTO;
    {t_AssignmentText}
    $DTOs += $DTO;
    }}

    return $DTOs; 
";

            return t_Text;
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
            var t_Repos = "";
            foreach(var t_Table in Schema.Tables)
            {
                t_Repos += $"$_CONTEXT | Add-Member -MemberType NoteProperty -Name {t_Table.Name} -Value $(New-{t_Table.Name}DTORepository($Con))" + '\n';
            }

            var t_Text = 
$@"
function New-StorageContext([System.Data.SQLite.SQLiteConnection] $Con)
{{
    $_CONTEXT = New-Object -TypeName psobject
    $_CONTEXT | Add-Member -MemberType NoteProperty -Name Connection -Value $Con

    {t_Repos}

    return $_CONTEXT;
}}
";


            return t_Text;
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_PSGen = new PSGenerator();


            var t_Files = t_PSGen.GenerateSource(Schema);

            t_Files.First().Content += _GenerateSchema(Schema) + _GenerateContext(Schema);

            //t_Files.Add(new ORMSourceFile { Name = Schema.Name + ".ps1", Content = _GenerateSchema(Schema) });

            return t_Files;
        }
    }
}
