using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Equality : BinaryOperator
{
    internal Equality(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Equality,
        "==",
        Type.Boolean,
        left.Type) { }
}
