using DatabaseORMGenerator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseORMGenerator
{
    public class CppSqlGenerator : IDTOGenerator
    {
        // Privates
        private string _GenerateSqlRepo(Table Table)
        {
            var t_UniqueColumn = _GetUniqueColumn(Table);
            return 
                "/* COMPUTER GENERATED CODE */" + '\n' +
                "#pragma once" + '\n' +
                "#include \"ISqlContract.h\"" + '\n' +
                $"#include \"{Table.Name}DTO.h\"" + '\n' +
                "#include <string>" + '\n' +
                "#include <memory>" + '\n' +
                "#include <vector>" + '\n' +
                "class " + Table.Name + "DTO" + "Repository" + '\n' +
                "{" + '\n' +
                "private:" + '\n' +
                "std::shared_ptr<ISqlContract> m_DB = nullptr;" + '\n' +
                "public:" + '\n' +
                $"{Table.Name + "DTO"}Repository(std::shared_ptr<ISqlContract> DB)" + '\n' +
                "{" + '\n' +
                "    m_DB = DB;" + '\n' +
                "}" + '\n' +
                _GenerateCreateFunction(Table) + '\n' +
                _GenerateReadFunction(Table) + '\n' +
                (t_UniqueColumn != null ? _GenerateDeleteFunction(Table) : "/* NO UNIQUE COLUMN FOUND. NO DELETE FUNCTION GENERATED */") + '\n' +
                (t_UniqueColumn != null ? _GenerateUpdateFunction(Table) : "/* NO UNIQUE COLUMN FOUND. NO UPDATE FUNCTION GENERATED */") + '\n' +
                "" + '\n' +
                "};";
        }

        private Column _GetUniqueColumn(Table Table)
        {
            var t_Unique = 
                Table.Columns
                .Where(C => C.Value.Property == COLUMN_PROPERTY_TYPE.UNIQUE)
                .Select(KV => KV.Value).FirstOrDefault();

            return t_Unique;
        }

        // CRUD - Operations
        private string _GenerateCreateFunction(Table Table)
        {
            var t_DataString = "std::string t_Values = \"\";" + '\n';

            foreach(var t_C in Table.Columns)
            {
                if (t_C.Value.Type == COLUMN_DATA_TYPE.STRING)
                    t_DataString += $"t_Values += DTO.{t_C.Value.Name} + \",\";" + '\n';
                else
                    t_DataString += $"t_Values += \"'\" + std::to_string(DTO.{t_C.Value.Name}) + \"',\";" + '\n';
            }

            t_DataString += "t_Values = t_Values.substr(0, t_Values.length() - 1);" + '\n';

            var t_ColumnStr = String.Join(",", Table.Columns.Select(C => C.Value.Name).ToArray<string>());
            var t_Text = $"void Create({Table.Name + "DTO"} DTO)" + '\n' +
            "{" + '\n' +
            t_DataString + '\n' +
            $"    m_DB->ExecuteStatement(\"INSERT INTO {Table.Name}({t_ColumnStr}) VALUES(\" + t_Values + \");\");" + '\n' +
            "}" + '\n';

            return t_Text;
        }

        private string _GenerateReadFunction(Table Table)
        {
            var t_SelectQuery = $"\"SELECT {String.Join(",", Table.Columns.Select(C => C.Value.Name).ToArray<string>())} FROM {Table.Name};\"";
            var t_DTOAssignment = "";

            foreach(var t_Col in Table.Columns)
            {
                var t_FuncType = "GetText";

                if (t_Col.Value.Type == COLUMN_DATA_TYPE.INTEGER)
                    t_FuncType = "GetInt";
                else if (t_Col.Value.Type == COLUMN_DATA_TYPE.FLOATING)
                    t_FuncType = "GetFloat";
                else
                    t_FuncType = "GetText";

                t_DTOAssignment += $"DTO.{t_Col.Value.Name} = t_Row->{t_FuncType}({t_Col.Key});" + '\n';
            }

            var t_FunctionText =
@"std::vector<[DTO_NAME]> Read()
{
	std::vector<[DTO_NAME]> t_DTOs{};
	auto t_Statement = m_DB->ExecuteQuery([SELECT_QUERY]);

    ISqlRow* t_Row = nullptr;
    while ((t_Row = t_Statement->GetNextRow()) != nullptr)
    {
        [DTO_NAME] DTO {};
        [DTO_VAR_ASSIGNMENT]
        t_DTOs.push_back(DTO);
    }

    return t_DTOs;
}"
.Replace("[DTO_NAME]", Table.Name + "DTO")
.Replace("[SELECT_QUERY]", t_SelectQuery)
.Replace("[DTO_VAR_ASSIGNMENT]", t_DTOAssignment);

            return t_FunctionText;
        }

        private string _GenerateUpdateFunction(Table Table)
        {
            if (Table.Columns.Count <= 1) return "/* TABLE HAS ONLY A SINGLE UNIQUE COLUMN. NO UPDATE FUNCTION GENERATED.*/";

            var t_UniqueColumn = _GetUniqueColumn(Table);
            var t_SetText = "";
            var t_NonUniqueColumns = Table.Columns.Where(KV => KV.Value.Property != COLUMN_PROPERTY_TYPE.UNIQUE).Select(KV => KV.Value).ToList();
            foreach(var t_C in t_NonUniqueColumns)
            {
                t_SetText += $"+ std::to_string(DTO.{t_C.Name}) + \",\" ";
            }
            t_SetText = t_SetText.Substring(0, t_SetText.Length - " + \",\" ".Length);

            var t_FunctionText = 
            $"void Update({Table.Name + "DTO"} DTO)" + '\n' +
            "{" + '\n' +
            $"\tm_DB->ExecuteStatement(\"UPDATE {Table.Name} SET \" " + t_SetText + $"+ \" WHERE {t_UniqueColumn.Name} = \" + std::to_string(DTO.{t_UniqueColumn.Name}) + \";\"" + ");" + '\n' +
            "}" + '\n';

            return t_FunctionText;
        }

        private string _GenerateDeleteFunction(Table Table)
        {
            var t_UniqueColumn = _GetUniqueColumn(Table);
            var t_FunctionText =
            $"void Delete({Table.Name + "DTO"} DTO)" + '\n' +
            "{" + '\n' +
            $"\tm_DB->ExecuteStatement(\"DELETE FROM {Table.Name} WHERE {t_UniqueColumn.Name} = \" + std::to_string(DTO.{t_UniqueColumn.Name}) + \";\");" + '\n' +
            "}" + '\n';

            return t_FunctionText;
        }

        private string _GenerateSqlInterface()
        {
            return
@"
#pragma once
#include <string>

class ISqlRow
{
public:
	virtual int GetInt(int Id) = 0;
	virtual std::string GetText(int Id) = 0;
	virtual float GetFloat(int Id) = 0;
	virtual bool GetBool(int Id) = 0;

	virtual ~ISqlRow() {};
};

class ISqlStatement
{
public:
	virtual ISqlRow *GetNextRow() = 0;
	virtual ~ISqlStatement() {};
};

class ISqlContract
{
public:

	virtual void ExecuteStatement(std::string Statement) = 0;
	virtual std::unique_ptr<ISqlStatement> ExecuteQuery(std::string Query) = 0;

	virtual ~ISqlContract() {};
};
";
        }

        // Interface
        public List<ORMSourceFile> GenerateSource(Schema Schema)
        {
            var t_CppGenerator = new CppGenerator();
            var t_Files = t_CppGenerator.GenerateSource(Schema);

            foreach (var t_Table in Schema.Tables)
            {
                var t_File = new ORMSourceFile { Name = t_Table.Name + "DTO" + "Repository.h", Content = _GenerateSqlRepo(t_Table) };
                t_Files.Add(t_File);
            }

            t_Files.Add( new ORMSourceFile { Name = "ISqlContract.h", Content = _GenerateSqlInterface() });

            return t_Files;
        }
    }
}
