using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class As : UnaryOperator
{
    public Type TargetType { get; private init; }

    internal As(Expression value, Type targetType) : base(
        value,
        OperatorType.As,
        "as",
        targetType)
    {
        TargetType = targetType;
    }

    public override string ToString()
    {
        return $"{Operand} {Sign} {TargetType.Name}";
    }
}
