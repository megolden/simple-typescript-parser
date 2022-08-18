using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Remainder : BinaryOperator
{
    internal Remainder(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Remainder,
        "%",
        Type.Number,
        Type.Number) { }
}
