using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseOr : BinaryOperator
{
    internal BitwiseOr(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseOr,
        "|",
        Type.Number,
        Type.Number) { }
}
