using System;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public class FunctionParameter
{
    public string Name { get; private init; }
    public Type Type { get; private init; }
    public bool IsOptional { get; private init; }
    public bool IsRest { get; private init; }

    public FunctionParameter(string name, Type type, bool isOptional = false, bool isRest = false)
    {
        if (isRest && !type.IsArray)
            throw new ArgumentException($"rest parameters type must be array: {name}");

        Name = name;
        Type = type;
        IsOptional = isOptional;
        IsRest = isRest;
    }

    public override string ToString()
    {
        return $"{(IsRest ? "..." : "")}{Name}{(IsOptional && !IsRest ? "?" : "")}: {Type}";
    }
}
