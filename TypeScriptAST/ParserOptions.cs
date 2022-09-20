using TypeScriptAST.Declarations;
using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST;

public struct ParserOptions
{
    public TypeSystem TypeSystem { get; } = new();

    public ParserOptions(params ModuleDef[] modules)
    {
        TypeSystem.AddModules(modules);
    }

    public static ParserOptions GetDefault()
    {
        return new ParserOptions(new GlobalModule());
    }
}
