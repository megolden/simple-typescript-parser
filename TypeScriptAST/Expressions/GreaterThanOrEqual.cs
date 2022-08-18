using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class GreaterThanOrEqual : BinaryOperator
{
    internal GreaterThanOrEqual(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.GreaterThanOrEqual,
        ">=",
        Type.Boolean,
        Type.Number) { }
}
