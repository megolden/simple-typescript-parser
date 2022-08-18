using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class UnaryPlus : UnaryOperator
{
    internal UnaryPlus(Expression operand) : base(
        operand,
        OperatorType.UnaryPlus,
        "+",
        Type.Number,
        Type.Number) { }
}
