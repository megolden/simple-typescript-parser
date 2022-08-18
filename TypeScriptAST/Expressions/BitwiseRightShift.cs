using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseRightShift : BinaryOperator
{
    internal BitwiseRightShift(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseRightShift,
        ">>",
        Type.Number,
        Type.Number) { }
}
