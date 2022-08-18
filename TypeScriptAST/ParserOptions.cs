using System.Collections.Generic;
using TypeScriptAST.Declarations;

namespace TypeScriptAST;

public struct ParserOptions
{
    public ParserOptions()
    {
    }

    public List<ModuleDef> Modules { get; set; } = new();

    public static ParserOptions GetDefault()
    {
        var options = new ParserOptions();
        options.Modules.Add(new GlobalModule());
        return options;
    }
}
