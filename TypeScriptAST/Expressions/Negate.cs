using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Negate : UnaryOperator
{
    internal Negate(Expression operand) : base(
        operand,
        OperatorType.UnaryNegation,
        "-",
        Type.Number,
        Type.Number) { }
}
