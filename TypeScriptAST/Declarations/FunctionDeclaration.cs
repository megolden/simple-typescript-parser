using System.Collections.Generic;
using String = System.String;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public class FunctionDeclaration : Declaration, IMemberInfo
{
    public Type Type { get; set; } = Type.Void;
    public List<FunctionParameter> Parameters { get; set; } = new();

    public override string ToString()
    {
        return $"{FullName}({String.Join(", ", Parameters)}): {Type}";
    }
}
