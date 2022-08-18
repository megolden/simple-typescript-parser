using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class LetDeclaration : Declaration, IMemberInfo
{
    public Type Type { get; set; } = Type.Any;

    public override string ToString()
    {
        return $"{FullName}: {Type}";
    }
}
