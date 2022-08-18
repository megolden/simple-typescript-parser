using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class LogicalOr : BinaryOperator
{
    internal LogicalOr(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.LogicalOr,
        "||",
        Type.Boolean) { }
}
