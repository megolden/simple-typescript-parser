using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class GreaterThan : BinaryOperator
{
    internal GreaterThan(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.GreaterThan,
        ">",
        Type.Boolean,
        Type.Number) { }
}
