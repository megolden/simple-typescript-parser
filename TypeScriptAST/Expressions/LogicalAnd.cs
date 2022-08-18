using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class LogicalAnd : BinaryOperator
{
    internal LogicalAnd(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.LogicalAnd,
        "&&",
        Type.Boolean) { }
}
