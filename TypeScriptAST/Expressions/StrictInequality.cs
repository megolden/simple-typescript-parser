using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class StrictInequality : BinaryOperator
{
    internal StrictInequality(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.StrictInequality,
        "!==",
        Type.Boolean,
        left.Type) { }
}
