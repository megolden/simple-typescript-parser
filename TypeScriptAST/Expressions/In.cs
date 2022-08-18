using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class In : BinaryOperator
{
    internal In(Expression property, Expression obj) : base(
        property,
        obj,
        OperatorType.In,
        "in",
        Type.Boolean) { }
}
