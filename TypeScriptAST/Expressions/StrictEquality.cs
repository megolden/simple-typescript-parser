using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class StrictEquality : BinaryOperator
{
    internal StrictEquality(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.StrictEquality,
        "===",
        Type.Boolean,
        left.Type) { }
}
