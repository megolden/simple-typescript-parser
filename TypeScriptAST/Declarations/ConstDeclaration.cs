using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class ConstDeclaration : Declaration, IMemberInfo
{
    public Type Type { get; set; } = Type.Any;

    public override string ToString()
    {
        return $"{FullName}: {Type}";
    }
}
