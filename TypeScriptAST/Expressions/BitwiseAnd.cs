using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseAnd : BinaryOperator
{
    internal BitwiseAnd(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseAnd,
        "&",
        Type.Number,
        Type.Number) { }
}
