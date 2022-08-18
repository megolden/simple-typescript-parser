using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Inequality : BinaryOperator
{
    internal Inequality(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Inequality,
        "!=",
        Type.Boolean,
        left.Type) { }
}
