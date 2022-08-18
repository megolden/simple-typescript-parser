using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class BitwiseXor : BinaryOperator
{
    internal BitwiseXor(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.BitwiseXor,
        "^",
        Type.Number,
        Type.Number) { }
}
