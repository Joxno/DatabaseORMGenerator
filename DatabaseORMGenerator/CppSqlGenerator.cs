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
            return 
                "/* COMPUTER GENERATED CODE */" + '\n' +
                "#pragma once" + '\n' +
                "#include \"ISqlContract.h\"" + '\n' +
                $"#include \"{Table.Name}.h\"" +
                "#include <string>" + '\n' +
                "#include <memory>" + '\n' +
                "#include <vector>" + '\n' +
                "class " + Table.Name + "Repository" + '\n' +
                "{" + '\n' +
                "private:" + '\n' +
                "std::shared_ptr<ISqlContract> m_DB = nullptr;" + '\n' +
                "public:" + '\n' +
                $"{Table.Name}Repository(std::shared_ptr<ISqlContract> DB)" + '\n' +
                "{" + '\n' +
                "    m_DB = DB;" + '\n' +
                "}" + '\n' +
                _GenerateCreateFunction(Table) + '\n' +
                _GenerateReadFunction(Table) + '\n' +
                "" + '\n' +
                "}" + '\n' +
                "";
        }

        // CRUD - Operations
        private string _GenerateCreateFunction(Table Table)
        {
            var t_DataString = "string t_Values = \"\";" + '\n';

            foreach(var t_C in Table.Columns)
            {
                if (t_C.Value.Type == COLUMN_DATA_TYPE.STRING)
                    t_DataString += $"t_Values += DTO.{t_C.Value.Name} + \",\";" + '\n';
                else
                    t_DataString += $"t_Values += \"'\" + std::to_string(DTO.{t_C.Value.Name}) + \"',\";" + '\n';
            }

            t_DataString += "t_Values = t_Values.substr(0, t_Values.length - 1);" + '\n';

            var t_ColumnStr = String.Join(",", Table.Columns.Select(C => C.Value.Name).ToArray<string>());
            var t_Text = $"void Create({Table.Name} DTO)" + '\n' +
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
.Replace("[DTO_NAME]", Table.Name)
.Replace("[SELECT_QUERY]", t_SelectQuery)
.Replace("[DTO_VAR_ASSIGNMENT]", t_DTOAssignment);

            return t_FunctionText;
        }

        private string _GenerateUpdateFunction(Table Table)
        {
            return "";
        }

        private string _GenerateDeleteFunction(Table Table)
        {
            return "";
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
	                    int GetInt(int Id);
	                    std::string GetText(int Id);
	                    float GetFloat(int Id);
	                    bool GetBool(int Id);

	                    ~ISqlRow() = delete;
                    };

                    class ISqlStatement
                    {
                    public:
	                    ISqlRow *GetNextRow();
	                    ~ISqlStatement() = delete;
                    };

                    class ISqlContract
                    {
                    public:

	                    virtual void ExecuteStatement(std::string Statement);
	                    virtual ISqlStatement *ExecuteQuery(std::string Query);

	                    ~ISqlContract() = delete;
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
                var t_File = new ORMSourceFile { Name = t_Table.Name + "Repository.h", Content = _GenerateSqlRepo(t_Table) };
                t_Files.Add(t_File);
            }

            t_Files.Add( new ORMSourceFile { Name = "ISqlContract.h", Content = _GenerateSqlInterface() });

            return t_Files;
        }
    }
}
