using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Subtract : BinaryOperator
{
    internal Subtract(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Subtraction,
        "-",
        Type.Number,
        Type.Number) { }
}
