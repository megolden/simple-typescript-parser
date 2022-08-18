using System.Collections.Generic;
using String = System.String;

namespace TypeScriptAST.Declarations;

public class FunctionDefinition : MemberDefinition
{
    public const string ConstructorName = "constructor";

    public List<FunctionParameter> Parameters { get; set; } = new();

    public bool IsConstructor => Name == ConstructorName;

    public FunctionDefinition()
    {
        Type = Types.Type.Void;
    }

    public override string ToString()
    {
        return $"{Name}({String.Join(", ", Parameters)}): {Type}";
    }
}
