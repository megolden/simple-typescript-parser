using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class LessThan : BinaryOperator
{
    internal LessThan(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.LessThan,
        "<",
        Type.Boolean,
        Type.Number) { }
}
