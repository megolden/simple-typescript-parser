using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Divide : BinaryOperator
{
    internal Divide(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Division,
        "/",
        Type.Number,
        Type.Number) { }
}
