using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class LogicalNot : UnaryOperator
{
    internal LogicalNot(Expression operand) : base(
        operand,
        OperatorType.LogicalNot,
        "!",
        Type.Boolean) { }
}
