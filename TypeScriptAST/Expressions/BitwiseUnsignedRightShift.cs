using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseUnsignedRightShift : BinaryOperator
{
    internal BitwiseUnsignedRightShift(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseUnsignedRightShift,
        ">>>",
        Type.Number,
        Type.Number) { }
}
