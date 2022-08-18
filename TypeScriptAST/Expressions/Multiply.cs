using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Multiply : BinaryOperator
{
    internal Multiply(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Multiplication,
        "*",
        Type.Number,
        Type.Number) { }
}
