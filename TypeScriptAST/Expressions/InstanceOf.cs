using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class InstanceOf : UnaryOperator
{
    public Type TargetType { get; private init; }

    internal InstanceOf(Expression obj, Type targetType) : base(
        obj,
        OperatorType.InstanceOf,
        "instanceof",
        Type.Boolean)
    {
        TargetType = targetType;
    }

    public override string ToString()
    {
        return $"{Operand} {Sign} {TargetType.Name}";
    }
}
