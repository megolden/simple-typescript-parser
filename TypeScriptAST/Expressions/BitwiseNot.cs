using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseNot : UnaryOperator
{
    internal BitwiseNot(Expression operand) : base(
        operand,
        OperatorType.BitwiseNot,
        "~",
        Type.Number,
        Type.Number) { }
}
