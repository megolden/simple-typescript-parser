using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Exponent : BinaryOperator
{
    internal Exponent(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Exponentiation,
        "**",
        Type.Number,
        Type.Number) { }
}
