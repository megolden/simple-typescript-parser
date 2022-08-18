using System;
using TypeScriptAST.Declarations.Types;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Expressions;

public class MemberIndex : Operator
{
    public Expression Instance { get; private init; }
    public Expression Index { get; private init; }
    public bool IsOptional { get; private init; }

    internal MemberIndex(Expression instance, Expression index, bool isOptional)
        : base(OperatorType.MemberAccess, "[]", ResolveType(instance.Type, index))
    {
        Instance = instance;
        Index = index;
        IsOptional = isOptional;
    }

    private static Type ResolveType(Type instanceType, Expression index)
    {
        if (instanceType is Declarations.Types.Array arrayType)
            return arrayType.UnderlyingType;

        if (index is Literal { Value: { } value } && value.ToString() is { } name)
        {
            if (TypeResolver.GetMember(instanceType, name)?.Type is { } type)
                return type;

            if (!Type.Any.IsAssignableFrom(instanceType))
                throw new MissingMemberException(instanceType.Name, name);
        }

        return Type.Any;
    }

    public override string ToString()
    {
        return $"{Instance}{(IsOptional ? "?." : "")}[{Index}]";
    }
}
