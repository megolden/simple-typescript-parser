using System;
using TypeScriptAST.Declarations.Types;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public class FunctionParameter : IEquatable<FunctionParameter>
{
    public string Name { get; }
    public Type Type { get; }
    public bool IsOptional { get; }
    public bool IsRest { get; }

    public FunctionParameter(string name, Type type, bool isOptional = false, bool isRest = false)
    {
        if (isRest && !TypeSystem.IsArray(type))
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

    public override bool Equals(object? other)
    {
        return other is FunctionParameter that &&
               this.Name == that.Name &&
               this.Type == that.Type &&
               this.IsOptional == that.IsOptional &&
               this.IsRest == that.IsRest;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type, IsOptional, IsRest);
    }

    bool IEquatable<FunctionParameter>.Equals(FunctionParameter? other) => this.Equals(other);
}
