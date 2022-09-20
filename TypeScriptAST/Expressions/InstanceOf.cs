using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class InstanceOf : UnaryOperator
{
    public Expression TargetType { get; }

    internal InstanceOf(Expression obj, Expression targetType) : base(
        obj,
        OperatorType.InstanceOf,
        "instanceof",
        Type.Boolean)
    {
        TargetType = targetType;
    }

    public override string ToString()
    {
        return $"{Operand} {Sign} {TargetType}";
    }
}
