using System.Linq;
using TypeScriptAST.Declarations.Types;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Expressions;

public class MemberIndex : Operator
{
    public Expression Instance { get; }
    public Expression Index { get; }
    public bool IsOptional { get; }

    internal MemberIndex(Expression instance, Expression index, bool isOptional)
        : base(OperatorType.MemberAccess, "[]", ResolveType(instance.Type, index))
    {
        Instance = instance;
        Index = index;
        IsOptional = isOptional;
    }

    private static Type ResolveType(Type instanceType, Expression index)
    {
        if (TypeSystem.GetElementType(instanceType) is { } elementType)
            return elementType;

        if (index is Literal { Value: { } value } && value.ToString() is { } name)
        {
            if (TypeSystem.GetMember(instanceType, name).FirstOrDefault() is { Type: { } type })
                return type;
        }

        return Type.Any;
    }

    public override string ToString()
    {
        return $"{Instance}{(IsOptional ? "?." : "")}[{Index}]";
    }
}
