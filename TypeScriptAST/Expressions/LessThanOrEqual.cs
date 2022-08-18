using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class LessThanOrEqual : BinaryOperator
{
    internal LessThanOrEqual(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.LessThanOrEqual,
        "<=",
        Type.Boolean,
        Type.Number) { }
}
