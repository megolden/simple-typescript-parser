using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseLeftShift : BinaryOperator
{
    internal BitwiseLeftShift(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseLeftShift,
        "<<",
        Type.Number,
        Type.Number) { }
}
